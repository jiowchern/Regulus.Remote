namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon
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


        public static IProtocol CreateCase2()
        {
            IProtocol protocol = null;
            _CreateCase2(ref protocol);
            return protocol;
        }

        [Remote.Protocol.Creater]
        static partial void _CreateCase2(ref IProtocol protocol);
    }
}
