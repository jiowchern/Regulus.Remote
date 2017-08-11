using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Network.RUDP
{
    public class SegmentStream
    {
        private readonly List<SocketMessage> _Packages;
        private int _FirstPackageOffset;
        public int Length { get { return _Packages.Sum(p => p.GetPayloadLength()); } }
        public int Count { get { return _Packages.Count; } }

        
        public SegmentStream(SocketMessage[] messages)
        {
            _Packages = messages.ToList();
            _FirstPackageOffset = 0;
        }        

       
        public int Read(byte[] buffer, int target_offset, int read_count)
        {
            var count = read_count < buffer.Length ? read_count : buffer.Length;
            int pkgIndex = 0;
            int readed = 0;
            int removeCount = 0;
            while (pkgIndex < _Packages.Count && count > 0)
            {
                var pkg = _Packages[pkgIndex];
                var payLoadLength = pkg.GetPayloadLength();
                var copyLength = payLoadLength < count ? payLoadLength : count;


                var copyCount = pkg.ReadPayload(_FirstPackageOffset, buffer, target_offset, copyLength);

                if (copyCount + _FirstPackageOffset == payLoadLength)
                {
                    _FirstPackageOffset = 0;
                    removeCount++;
                }
                else
                {
                    _FirstPackageOffset += copyCount;
                }
                target_offset += copyCount;
                count -= copyCount;
                pkgIndex++;
                readed += copyCount;
            }

            _Packages.RemoveRange(0, removeCount);

            return readed;
        }


        public byte this[int i]
        {
            get
            {
                return _Get(i);
            }
        }

        private byte _Get(int offset)
        {
            
            for (int i = 0; i < Length; i++)
            {
                var package = _Packages[i];
                var len = package.GetPayloadLength();
                if (offset < len)
                {
                    var readed = new Byte[1];
                    package.ReadPayload(readed, offset);
                    return readed[0];
                }            
                offset -= len;
            }

            return 0;
        }

        public void Add(SocketMessage message)
        {
            _Packages.Add(message);
            
        }

        
    }
}