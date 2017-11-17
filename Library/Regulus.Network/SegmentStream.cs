using System.Collections.Generic;
using System.Linq;
using Regulus.Network.Package;

namespace Regulus.Network
{
    public class SegmentStream
    {
        private readonly List<SocketMessage> m_Packages;
        private int m_FirstPackageOffset;
        public int Length { get { return m_Packages.Sum(P => P.GetPayloadLength()); } }
        public int Count => m_Packages.Count;


        public SegmentStream(SocketMessage[] Messages)
        {
            m_Packages = Messages.ToList();
            m_FirstPackageOffset = 0;
        }        

       
        public int Read(byte[] Buffer, int TargetOffset, int ReadCount)
        {
            var count = ReadCount < Buffer.Length ? ReadCount : Buffer.Length;
            var pkgIndex = 0;
            var readed = 0;
            var removeCount = 0;
            while (pkgIndex < m_Packages.Count && count > 0)
            {
                var pkg = m_Packages[pkgIndex];
                var payLoadLength = pkg.GetPayloadLength();
                var copyLength = payLoadLength < count ? payLoadLength : count;


                var copyCount = pkg.ReadPayload(m_FirstPackageOffset, Buffer, TargetOffset, copyLength);

                if (copyCount + m_FirstPackageOffset == payLoadLength)
                {
                    m_FirstPackageOffset = 0;
                    removeCount++;
                }
                else
                {
                    m_FirstPackageOffset += copyCount;
                }
                TargetOffset += copyCount;
                count -= copyCount;
                pkgIndex++;
                readed += copyCount;
            }

            m_Packages.RemoveRange(index: 0, count: removeCount);

            return readed;
        }


        public byte this[int I] => Get(I);

        private byte Get(int Offset)
        {
            
            for (var i = 0; i < Length; i++)
            {
                var package = m_Packages[i];
                var len = package.GetPayloadLength();
                if (Offset < len)
                {
                    var readed = new byte[1];
                    package.ReadPayload(readed, Offset);
                    return readed[0];
                }            
                Offset -= len;
            }

            return 0;
        }

        public void Add(SocketMessage Message)
        {
            m_Packages.Add(Message);
            
        }

        
    }
}