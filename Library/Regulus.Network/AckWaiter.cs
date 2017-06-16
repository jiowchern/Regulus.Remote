using System;
using System.Collections.Generic;

namespace Regulus.Network.RUDP
{
    public class AckWaiter
    {
        

        private readonly Dictionary<uint, long> _Packages;
        public AckWaiter()
        {
            _Packages = new Dictionary<uint, long>();        
        }

        public void PushWait(uint package_id, long end_ticks )
        {
            _Packages.Add(package_id , end_ticks);
        }

        public void EraseReply(uint package)
        {

            _Packages.Remove(package);
        }

        public void PopTimeout(long ticks, ref List<uint> packages)
        {

            foreach (var package in _Packages)
            {
                if (package.Value > ticks)
                {
                    packages.Add(package.Key);
                }
            }

            foreach (var package in packages)
            {
                _Packages.Remove(package);
            }
        }
    }
}