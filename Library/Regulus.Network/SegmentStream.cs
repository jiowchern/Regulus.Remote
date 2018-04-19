using System.Collections.Generic;
using System.Linq;
using Regulus.Network.Package;

namespace Regulus.Network
{
    public class SegmentStream
    {
        private readonly List<SocketMessage> m_Packages;
        private int m_FirstPackageOffset;
        public int Length { get { return m_Packages.Sum(p => p.GetPayloadLength()); } }
        public int Count { get { return m_Packages.Count; } }


        public SegmentStream(SocketMessage[] Messages)
        {
            m_Packages = Messages.ToList();
            m_FirstPackageOffset = 0;
        }        

       
        public int Read(byte[] buffer, int offset, int buffer_count)
        {
            var count = buffer_count < buffer.Length ? buffer_count : buffer.Length;
            var pkgIndex = 0;
            var readed = 0;
            var removeCount = 0;
            while (pkgIndex < m_Packages.Count && count > 0)
            {
                var pkg = m_Packages[pkgIndex];
                var payLoadLength = pkg.GetPayloadLength();
                var copyLength = payLoadLength < count ? payLoadLength : count;


                var copyCount = pkg.ReadPayload(m_FirstPackageOffset, buffer, offset, copyLength);

                if (copyCount + m_FirstPackageOffset == payLoadLength)
                {
                    m_FirstPackageOffset = 0;
                    removeCount++;
                }
                else
                {
                    m_FirstPackageOffset += copyCount;
                }
                offset += copyCount;
                count -= copyCount;
                pkgIndex++;
                readed += copyCount;
            }

            m_Packages.RemoveRange(index: 0, count: removeCount);

            return readed;
        }


        public byte this[int i] {get { return Get(i); } }

        private byte Get(int offset)
        {
            
            for (var i = 0; i < Length; i++)
            {
                var package = m_Packages[i];
                var len = package.GetPayloadLength();
                if (offset < len)
                {
                    var readed = new byte[1];
                    package.ReadPayload(readed, offset);
                    return readed[0];
                }            
                offset -= len;
            }

            return 0;
        }

        public void Add(SocketMessage Message)
        {
            m_Packages.Add(Message);
            
        }

        
    }
}