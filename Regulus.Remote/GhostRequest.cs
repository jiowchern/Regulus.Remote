namespace Regulus.Remote
{
    public interface IOpCodeExchangeable
    {
        void Request(ClientToServerOpCode code, byte[] args);
        event System.Action<ServerToClientOpCode, byte[]> ResponseEvent;
    }
}
