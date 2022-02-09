
using Regulus.Remote;
public static partial class ProtocolProviderCase3
{
    public static IProtocol CreateCase3()
    {
        IProtocol protocol = null;
        _CreateCase3(ref protocol);
        return protocol;
    }

    [Regulus.Remote.Protocol.Creator]
    static partial void _CreateCase3(ref IProtocol protocol);
}