using System;

namespace Regulus.Profiles.StandaloneAllFeature.Protocols
{
    public static partial class ProtocolProvider
    {
        public static Regulus.Remote.IProtocol Create()
        {
            Regulus.Remote.IProtocol protocol = null;
            _Create(ref protocol);
            return protocol;
        }

        [Remote.Protocol.Creater]
        static partial void _Create(ref Regulus.Remote.IProtocol protocol);


    }
}
