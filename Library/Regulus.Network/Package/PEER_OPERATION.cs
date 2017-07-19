namespace Regulus.Network.RUDP
{
    public enum PEER_OPERATION : byte
    {
        NONE,
        CLIENTTOSERVER_HELLO1,
        SERVERTOCLIENT_HELLO1,
        CLIENTTOSERVER_HELLO2,
        REQUEST_DISCONNECT,
        TRANSMISSION,
            
    }
}