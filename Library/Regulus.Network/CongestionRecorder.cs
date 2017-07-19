using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Regulus.Network.RUDP
{
    public class CongestionRecorder
    {
        private readonly int _HungryLimit;

        class Item
        {
            public readonly SocketMessage Message;
            public readonly long EndTicks;
            public int Hungry { get;private set; }
            public Item(SocketMessage message, long end_ticks)
            {
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
        public CongestionRecorder(int hungry_limit)
        {
            _HungryLimit = hungry_limit;
            _Items = new Dictionary<ushort, Item>();
        }

        public int Count { get { return _Items.Count; } }

        public void PushWait(SocketMessage message, long timeout_ticks )
        {
            var item = new Item(message , timeout_ticks);
            _Items.Add(item.Message.GetSeq(), item);
        }
        

        public void Reply(ushort package)
        {
            _Items.Remove(package);
        }


        public void ReplyUnder(ushort package_id)
        {
            var pkg = _Items.Values.FirstOrDefault(item => item.Message.GetSeq() == package_id);
            if (pkg != null)
            {
                foreach (var replyId in _Items.Values.Where(item => item.EndTicks <= pkg.EndTicks).Select(item => item.Message.GetSeq()).ToArray())
                {
                    Reply(replyId);
                }
                
            }
        }

        public List<SocketMessage> PopLost(long ticks)
        {

            List<SocketMessage> packages = new List<SocketMessage>();
            foreach (var item in _Items.Values)
            {
                if (item.IsTimeout(ticks)  )
                {
                    packages.Add(item.Message);
                }
                else if (item.Hungry > _HungryLimit)
                {
                    packages.Add(item.Message);
                }
            }

            foreach (var package in packages)
            {
                _Items.Remove(package.GetSeq());
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
    }
}