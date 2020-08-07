namespace Regulus.Remote
{
    public interface IResponseQueue
    {
        void Push(ServerToClientOpCode code, byte[] data);
    }
}
