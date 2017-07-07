using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Network
{
    public static class Config
    {

        static Config()
        {            
            HostListenTimeout = Timestamp.OneSecondTicks * 300;
            AgentConnectTimeout = Timestamp.OneSecondTicks * 300;
            TransmitterTimeout = Timestamp.OneSecondTicks * 300;
        }

        public static readonly long AgentConnectTimeout;
        public static readonly long HostListenTimeout;
        public static readonly long TransmitterTimeout;
        public const int PackageSize = 548;
    }
}
