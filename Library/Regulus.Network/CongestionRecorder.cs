using System.Collections.Generic;
using System.Linq;
using Regulus.Network.Package;

namespace Regulus.Network
{
    public class CongestionRecorder
    {
        private readonly int m_HungryLimit;
        private int m_Capacity;

        private class Item
        {
            public readonly SocketMessage Message;
            public readonly long EndTicks;
            public readonly long StartTicks;
            public int Hungry { get;private set; }
            public Item(SocketMessage message,long start_ticks, long end_ticks)
            {
                StartTicks = start_ticks;
                Message = message;
                EndTicks = end_ticks;
            }

            

            public bool IsTimeout(long Ticks)
            {
                return EndTicks <= Ticks;
            }

            public void Padding()
            {
                Hungry++;
            }
        }
        private readonly Dictionary<ushort, Item> m_Items;

        private readonly RetransmissionTimeOut m_Rto;
        

        public CongestionRecorder(int HungryLimit)
        {
            m_HungryLimit = HungryLimit;
            m_Items = new Dictionary<ushort, Item>();
            m_Rto = new RetransmissionTimeOut();
        }

        public int Count { get{ return m_Items.Count; } }
        public long Srtt {
            get { return m_Rto.Rtt; }
        } 
        public long Rto { get { return m_Rto.Value; } }

        public long LastRtt { get; private set; }
        public long LastRto { get; private set; }


        public void PushWait(SocketMessage Message, long TimeTicks )
        {
            var item = new Item(Message , TimeTicks , TimeTicks + m_Rto.Value);
            m_Items.Add(item.Message.GetSeq(), item);
        }
        

        public bool Reply(ushort Package,long TimeTicks,long TimeDelta)
        {
            Item item;
            if (m_Items.TryGetValue(Package, out item))
            {
                _Reply(Package, TimeTicks, TimeDelta, item);
                return true;
            }

            return false;
        }

        private void _Reply(ushort Package, long TimeTicks, long TimeDelta, Item Item)
        {
            m_Capacity++;
            var rtt = TimeTicks - Item.StartTicks;
            m_Rto.Update(rtt, TimeDelta);
            LastRtt = rtt;
            m_Items.Remove(Package);
        }


        public void ReplyBefore(ushort PackageId, long TimeTicks, long TimeDelta)
        {
            var pkg = m_Items.Values.FirstOrDefault(Item => Item.Message.GetSeq() == PackageId);
            if (pkg != null)
                foreach (var item in m_Items.Values.Where(Item => Item.EndTicks <= pkg.EndTicks).Select(Item => Item).ToArray())
                    _Reply(item.Message.GetSeq(), TimeTicks, TimeDelta, item);
        }

        public List<SocketMessage> PopLost(long Ticks,long Delta)
        {
            var count = m_Capacity;
            var packages = new List<SocketMessage>();
            foreach (var item in m_Items.Values)
            {
                
                if (item.IsTimeout(Ticks)  )
                {
                    var rto = Ticks - item.StartTicks;                    
                    m_Rto.Update(rto, Delta);
                    LastRtt = rto;
                    LastRto = rto;
                    packages.Add(item.Message);

                    
                }
                else if (item.Hungry > m_HungryLimit)
                {
                    var rto = Ticks - item.StartTicks;
                    
                    m_Rto.Update(rto, Delta);
                    LastRtt = rto;
                    LastRto = rto;
                    packages.Add(item.Message);
                }
                if (count-- == 0)
                    break;
            }


            if(packages.Count > 0 )
                m_Capacity /= 2;

            foreach (var package in packages)
            {
                m_Items.Remove(package.GetSeq());
                PushWait(package , Ticks);                
            }

            return packages;
        }

        public void Padding()
        {
            foreach (var itemsValue in m_Items.Values)
                itemsValue.Padding();
        }

        public void ReplyAfter(ushort Ack, uint Fields, long TimeTicks, long TimeDelta)
        {
            ushort mark = 1;
            for (ushort i = 0; i < 32; i++)
            {
                if ((mark & Fields) != 0)
                {
                    var id = (ushort) (Ack + i + 1);
                    Item item;
                    if (m_Items.TryGetValue(id, out item))
                        _Reply( id, TimeTicks, TimeDelta , item);
                }
                mark <<= 1;
            }
        }

        public bool IsFull()
        {
            return m_Items.Count > m_Capacity;
        }
    }
}