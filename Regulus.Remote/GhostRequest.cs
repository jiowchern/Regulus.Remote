namespace Regulus.Remote
{
    public interface IGhostRequest
    {
        void Request(ClientToServerOpCode code, byte[] args);
        event System.Action<ServerToClientOpCode, byte[]> ResponseEvent;
    }
}
