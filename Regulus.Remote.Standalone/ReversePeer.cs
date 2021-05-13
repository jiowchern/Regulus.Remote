using Regulus.Network;
using System.Threading.Tasks;
namespace Regulus.Remote.Standalone
{
    public class ReverseStream : Network.IStreamable
    {
        private readonly Stream _Peer;

        public ReverseStream(Stream peer)
        {
            this._Peer = peer;
        }


        IWaitableValue<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
            return _Peer.Pop(buffer, offset, count);

        }

        IWaitableValue<int> IStreamable.Send(byte[] buffer, int offset, int count)
        {
            return _Peer.Push(buffer, offset, count);
        }
    }
}
