namespace Regulus.Network.Package
{
    public enum PeerOperation : byte
    {
        Acknowledge,
        ClienttoserverHello1,
        ServertoclientHello1,
        ClienttoserverHello2,
        RequestDisconnect,
        Transmission
    }
}