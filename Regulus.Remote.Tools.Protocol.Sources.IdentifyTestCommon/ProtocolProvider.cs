namespace Regulus.Remote.Tools.Protocol.Sources.IdentifyTestCommon
{
    public static partial class ProtocolProvider
    {
        public static Regulus.Remote.IProtocol CreateCase1()
        {
            Regulus.Remote.IProtocol protocol = null;
            _CreateCase1(ref protocol);
            return protocol;
        }

        [Remote.Protocol.Creater]
        static partial void _CreateCase1(ref Regulus.Remote.IProtocol protocol);


    }
}


