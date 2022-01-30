namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon
{
    public class ProtocolProvider
    {
        [ProtocolProvideAttribute]
        public static Regulus.Remote.IProtocol Create()
        {
            return new Regulus.Remote.NewProtocol();
        }
    }
}
