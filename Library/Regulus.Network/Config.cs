using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Network.RUDP;

namespace Regulus.Network
{
    public class Config
    {

        public Config(int mtu)
        {
            MTU = mtu;
            PackageSize = MTU - IPv4HeadSize - UdpHeadSize;
        }
        
        public static readonly float Timeout = 30;
        public const int IPv4HeadSize = 20;
        public const int UdpHeadSize = 8;
        public readonly int MTU ;
        public readonly int PackageSize ; 


        public static readonly Config Default = new Config(1500);
    }
}
