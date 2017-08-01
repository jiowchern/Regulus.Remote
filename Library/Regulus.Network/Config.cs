using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Network.RUDP;

namespace Regulus.Network
{
    public static class Config
    {

        static Config()
        {            
            HostListenTimeout = 30;
            AgentConnectTimeout = 30;
            TransmitterTimeout = 30;
        }

        public static readonly long AgentConnectTimeout;
        public static readonly long HostListenTimeout;
        public static readonly long TransmitterTimeout;
        public const int IPv4HeadSize = 20;
        public const int UdpHeadSize = 8;
        public const int MTU = 576;
        public const int Cost = 32;
        //public const int MTU = 1500;
        public const int PackageSize = MTU - IPv4HeadSize - UdpHeadSize; 
    }
}
