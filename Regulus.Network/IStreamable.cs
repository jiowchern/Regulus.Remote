


namespace Regulus.Network
{
    public interface IStreamable
    {


        System.Threading.Tasks.Task<int> Receive(byte[] buffer, int offset, int count);
        System.Threading.Tasks.Task<int> Send(byte[] buffer, int offset, int count);

    }
}