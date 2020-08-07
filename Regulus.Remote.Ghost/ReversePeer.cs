using Regulus.Network;
using System.Threading.Tasks;
namespace Regulus.Remote.Standalone
{
    public class ReverseStream : Network.IStreamable
    {
        private readonly Stream peer;

        public ReverseStream(Stream peer)
        {
            this.peer = peer;
        }


        Task<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
            return peer.Pop(buffer, offset, count);

        }

        Task<int> IStreamable.Send(byte[] buffer, int offset, int count)
        {
            return peer.Push(buffer, offset, count);
        }
    }
}
