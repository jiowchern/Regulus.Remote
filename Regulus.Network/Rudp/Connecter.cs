using System;
using System.Net;
using System.Net.Sockets;

namespace Regulus.Network.Rudp
{
    
    public class Connector : IConnectable
    {
        private readonly Agent _Agent;
        private Regulus.Network.Socket _RudpSocket;

        public Connector(Agent Agent)
        {
            _Agent = Agent;
        }
        EndPoint IPeer.RemoteEndPoint {get { return _RudpSocket.EndPoint; } }

        EndPoint IPeer.LocalEndPoint {get { return _RudpSocket.EndPoint; } }

        bool IPeer.Connected {get{return _RudpSocket.Status == PeerStatus.Transmission; } }

        System.Threading.Tasks.Task<int> IPeer.Receive(byte[] ReadedByte, int Offset, int Count)
        {
            return _RudpSocket.Receive(ReadedByte, Offset, Count);
        }

        System.Threading.Tasks.Task<int> IPeer.Send(byte[] Buffer, int OffsetI, int BufferLength)
        {
            return _RudpSocket.Send(Buffer, OffsetI, BufferLength);
        }

        void IPeer.Close()
        {
            _RudpSocket.Disconnect();
        }

        System.Threading.Tasks.Task<bool> IConnectable.Connect(EndPoint Endpoint)
        {
            bool? result = null ;
            _RudpSocket = _Agent.Connect(Endpoint, r=> result = r);
            return System.Threading.Tasks.Task<bool>.Run(() => {

                var r = new Regulus.Utility.AutoPowerRegulator(new Utility.PowerRegulator());
                while(!result.HasValue)
                {
                    r.Operate();
                }
                return result.Value;
            });
            

        }
    }
}