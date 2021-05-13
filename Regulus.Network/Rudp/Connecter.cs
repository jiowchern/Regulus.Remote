using System.Net;
using System.Threading.Tasks;

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




        IWaitableValue<int> IStreamable.Receive(byte[] ReadedByte, int Offset, int Count)
        {
            return _RudpSocket.Receive(ReadedByte, Offset, Count);
        }

        IWaitableValue<int> IStreamable.Send(byte[] Buffer, int OffsetI, int BufferLength)
        {
            return _RudpSocket.Send(Buffer, OffsetI, BufferLength);
        }


        System.Threading.Tasks.Task<bool> IConnectable.Connect(EndPoint Endpoint)
        {
            bool? result = null;
            _RudpSocket = _Agent.Connect(Endpoint, r => result = r);
            return System.Threading.Tasks.Task<bool>.Run(() =>
            {

                Utility.AutoPowerRegulator r = new Regulus.Utility.AutoPowerRegulator(new Utility.PowerRegulator());
                while (!result.HasValue)
                {
                    r.Operate();
                }
                return result.Value;
            });


        }

        Task IConnectable.Disconnect()
        {
            throw new System.NotImplementedException();
        }
    }
}