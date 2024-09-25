namespace Regulus.Remote
{
    public interface IResponseQueue
    {
        void Push(ServerToClientOpCode code, Regulus.Memorys.Buffer buffer);
    }
}
