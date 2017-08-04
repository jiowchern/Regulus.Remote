using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Regulus.Network.RUDP
{
    public class CongestionRecorder
    {
        private readonly int _HungryLimit;
        private int _Count;
        class Item
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

            

            public bool IsTimeout(long ticks)
            {
                return EndTicks <= ticks;
            }

            public void Padding()
            {
                Hungry++;
            }
        }
        private readonly Dictionary<ushort, Item> _Items;

        private readonly RetransmissionTimeOut _RTO;
        

        public CongestionRecorder(int hungry_limit)
        {
            _HungryLimit = hungry_limit;
            _Items = new Dictionary<ushort, Item>();
            _RTO = new RetransmissionTimeOut();
        }

        public int Count { get { return _Items.Count; } }
        public long SRTT { get { return _RTO.RTT; } }
        public long RTO { get { return _RTO.Value; } }

        public long LastRTT { get; private set; }
        public long LastRTO { get; private set; }


        public void PushWait(SocketMessage message, long time_ticks )
        {
            var item = new Item(message , time_ticks , time_ticks + _RTO.Value);
            _Items.Add(item.Message.GetSeq(), item);
        }
        

        public bool Reply(ushort package,long time_ticks,long time_delta)
        {
            Item item;
            if (_Items.TryGetValue(package, out item))
            {
                _Reply(package, time_ticks, time_delta, item);
                return true;
            }

            return false;
        }

        private void _Reply(ushort package, long time_ticks, long time_delta, Item item)
        {
            _Count++;
            var rtt = time_ticks - item.StartTicks;
            _RTO.Update(rtt, time_delta);
            LastRTT = rtt;
            _Items.Remove(package);
        }


        public void ReplyBefore(ushort package_id, long time_ticks, long time_delta)
        {
            var pkg = _Items.Values.FirstOrDefault(item => item.Message.GetSeq() == package_id);
            if (pkg != null)
            {
                foreach (var item in _Items.Values.Where(item => item.EndTicks <= pkg.EndTicks).Select(item => item).ToArray())
                {                    
                    _Reply(item.Message.GetSeq(), time_ticks, time_delta, item);
                }
                
            }
            
        }

        public List<SocketMessage> PopLost(long ticks,long delta)
        {
            var count = _Count;
            List<SocketMessage> packages = new List<SocketMessage>();
            foreach (var item in _Items.Values)
            {
                
                if (item.IsTimeout(ticks)  )
                {
                    var rto = ticks - item.StartTicks;                    
                    _RTO.Update(rto, delta);
                    LastRTT = rto;
                    LastRTO = rto;
                    packages.Add(item.Message);

                    
                }
                else if (item.Hungry > _HungryLimit)
                {
                    var rto = ticks - item.StartTicks;
                    
                    _RTO.Update(rto, delta);
                    LastRTT = rto;
                    LastRTO = rto;
                    packages.Add(item.Message);
                }
                if (count-- == 0)
                    break;
            }


            if(packages.Count > 0 )
                _Count /= 2;

            foreach (var package in packages)
            {
                _Items.Remove(package.GetSeq());
                this.PushWait(package , ticks);                
            }

            return packages;
        }

        public void Padding()
        {
            foreach (var itemsValue in _Items.Values)
            {
                itemsValue.Padding();
            }
        }

        public void ReplyAfter(ushort ack, uint fields, long time_ticks, long time_delta)
        {
            ushort mark = 1;
            for (ushort i = 0; i < 32; i++)
            {
                if ((mark & fields) != 0)
                {
                    var id = (ushort) (ack + i + 1);
                    Item item;
                    if (_Items.TryGetValue(id, out item))
                    {
                        _Reply( id, time_ticks, time_delta , item);
                    }
                }
                mark <<= 1;
            }
        }

        public bool IsFull()
        {
            return _Items.Count > _Count;
        }
    }
}