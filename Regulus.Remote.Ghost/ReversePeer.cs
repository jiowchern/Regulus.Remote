using System.Threading.Tasks;
using Regulus.Network;
namespace Regulus.Remote.Standalone
{
    public class ReversePeer : Network.IStreamable
    {
        private readonly PeerStream peer;

        public ReversePeer(PeerStream peer)
        {
            this.peer = peer;
        }
		

        Task<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
			return peer.Pop(buffer , offset, count);

		}

        Task<int> IStreamable.Send(byte[] buffer, int offset, int count)
        {
			return peer.Push(buffer, offset, count);
		}
    }
}
