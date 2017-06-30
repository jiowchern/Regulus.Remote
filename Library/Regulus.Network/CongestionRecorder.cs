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
            public readonly SegmentPackage Package;
            public readonly long EndTicks;
            public int Hungry { get;private set; }
            public Item(SegmentPackage package, long end_ticks)
            {
                Package = package;
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
        private readonly Dictionary<uint, Item> _Items;
        public CongestionRecorder(int hungry_limit)
        {
            _HungryLimit = hungry_limit;
            _Items = new Dictionary<uint, Item>();
        }

        public void PushWait(SegmentPackage package, long end_ticks )
        {
            var item = new Item(package , end_ticks);
            _Items.Add(item.Package.Serial , item);
        }
        

        public void Reply(uint package)
        {
            _Items.Remove(package);
        }


        public void ReplyUnder(uint package_id)
        {
            var pkg = _Items.Values.FirstOrDefault(item => item.Package.Serial == package_id);
            if (pkg != null)
            {
                foreach (var replyId in _Items.Values.Where(item => item.EndTicks <= pkg.EndTicks).Select(item => item.Package.Serial))
                {
                    Reply(replyId);
                }
                
            }
        }

        public List<SegmentPackage> PopLost(long ticks)
        {

            List<SegmentPackage> packages = new List<SegmentPackage>();
            foreach (var item in _Items.Values)
            {
                if (item.IsTimeout(ticks)  )
                {
                    packages.Add(item.Package);
                }
                else if (item.Hungry > _HungryLimit)
                {
                    packages.Add(item.Package);
                }
            }

            foreach (var package in packages)
            {
                _Items.Remove(package.Serial);
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