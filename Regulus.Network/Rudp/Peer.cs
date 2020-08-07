namespace Regulus.Network.Rudp
{
    internal class Peer : IStreamable
    {
        private readonly Regulus.Network.Socket _RudpSocket;

        public Peer(Regulus.Network.Socket rudp_socket)
        {
            _RudpSocket = rudp_socket;

        }





        System.Threading.Tasks.Task<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
            return _RudpSocket.Receive(buffer, offset, count);

        }
        System.Threading.Tasks.Task<int> IStreamable.Send(byte[] buffer, int offset, int length)
        {
            return _RudpSocket.Send(buffer, offset, length);
        }


    }
}