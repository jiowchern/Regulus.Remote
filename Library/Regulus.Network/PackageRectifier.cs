using System.Collections.Generic;
using Regulus.Network.Package;

namespace Regulus.Network
{
    public class PackageRectifier
    {
        private readonly Dictionary<ushort, SocketMessage> m_DataPackages;

        private readonly Queue<SocketMessage> m_Packages;
        private ushort m_Serial;
        private uint m_SerialBitFields;

        public ushort Serial {get { return m_Serial; } }
        public uint SerialBitFields {get{ return m_SerialBitFields; } }
        public int Count { get { return m_DataPackages.Count; } }

        public PackageRectifier()
        {

            m_Packages = new Queue<SocketMessage>();
            m_DataPackages = new Dictionary<ushort, SocketMessage>();
            m_Serial = 0;
        }
        public bool PushPackage(SocketMessage SegmentMessage)
        {
            var seq = SegmentMessage.GetSeq();
            var exist = m_DataPackages.ContainsKey(seq) ;
            
            if (exist == false && SerialMoreRecent(seq) )
            {
                m_DataPackages.Add(seq, SegmentMessage);

                if (m_Serial == seq)
                    m_Serial = Rectify(m_Serial);


                uint mask = 1;
                for (uint i = 0; i < 32; i++)
                {
                    SocketMessage pkg;
                    if (m_DataPackages.TryGetValue((ushort)(m_Serial + i ), out pkg))
                        m_SerialBitFields = m_SerialBitFields | mask;

                    mask <<= 1;
                }

                return true;
            }
            return false;
        }

        private bool SerialMoreRecent(ushort serial)
        {
            return serial >= m_Serial &&
                   serial - m_Serial <= 0xFFFF / 2
                   ||
                   m_Serial >= serial &&
                   m_Serial - serial > 0xFFFF / 2;
        }

        public SocketMessage PopPackage()
        {
            lock (m_Packages)
            {
                if (m_Packages.Count > 0)
                    return m_Packages.Dequeue();
                return null;
            }

        }

        private ushort Rectify(ushort serial)
        {
            var removePackages = new List<ushort>();
            var index = serial;
            SocketMessage message;
            while (m_DataPackages.TryGetValue(index, out message))
            {
                lock (m_Packages)
                {
                    m_Packages.Enqueue(message);
                }
                
                removePackages.Add(index);
                index++;
            }
            foreach (var removePackage in removePackages)
                m_DataPackages.Remove(removePackage);

            return index;
        }
    }
}