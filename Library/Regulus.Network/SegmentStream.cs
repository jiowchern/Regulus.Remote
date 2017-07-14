using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Network.RUDP
{
    public class SegmentStream
    {
        private readonly List<SegmentPackage> _Packages;
        public int Length { get { return _Packages.Sum(p => p.GetPayloadLength()); } }

        public SegmentStream() 
        {
            _Packages = new List<SegmentPackage>();
        }
        public SegmentStream(SegmentPackage[] packages)
        {
            _Packages = packages.ToList();
            
        }        

       
        public int Read(int source_index , byte[] buffer, int target_offset, int read_count)
        {
            
            for (int i = 0; i < _Packages.Count; i++)
            {
                var pkg = _Packages[i];
                var payloadLength = pkg.GetPayloadLength();
                var payloadCanReadCount = payloadLength - source_index;
                if (payloadCanReadCount <= 0)
                {
                    source_index -= payloadLength;
                    continue;
                }
                    
                
                var sourceOffset = source_index;
                var targetOffset = target_offset;
                var readCount = payloadCanReadCount < read_count ? payloadCanReadCount : read_count;
                var readed = pkg.ReadPayload(sourceOffset, buffer, targetOffset, readCount);
                source_index = 0;
                target_offset += readed;
                read_count -= readed;
            }

            return 0;
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

        public void Add(SegmentPackage package)
        {
            _Packages.Add(package);
            
        }
    }
}