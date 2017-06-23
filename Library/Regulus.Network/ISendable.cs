namespace Regulus.Network.RUDP
{
    public interface ISendable
    {
        void Transport(SocketPackage package);
    }
}