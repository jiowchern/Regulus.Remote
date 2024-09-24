namespace Regulus.Remote
{
    public interface IOpCodeExchangeable
    {
        void Request(ClientToServerOpCode code, Regulus.Memorys.Buffer args);
        event System.Action<ServerToClientOpCode, Regulus.Memorys.Buffer> ResponseEvent;
    }
}
