using System;
using System.Text;
using Regulus.Network.RUDP;
using Regulus.Utility;
using Console = Regulus.Utility.Console;

namespace Regulus.Network.Tests.AgentApp
{
    internal class TransmissionStage : IStage
    {
        private readonly IPeer _Peer;
        private readonly Command _Command;
        private readonly Console.IViewer _Viewer;
        public event Action DisconnectEvent;

        public TransmissionStage(IPeer peer, Command command , Console.IViewer viewer)
        {
            _Peer = peer;
            _Command = command;
            _Viewer = viewer;
        }

        void IStage.Enter()
        {
            _Command.RegisterLambda<TransmissionStage , string>(this, (ins , message) => ins.Send(message) );
        }

        void IStage.Leave()
        {
            _Command.Unregister("Send");
        }

        void IStage.Update()
        {
            if (_Peer.Status == PEER_STATUS.CLOSE)
                DisconnectEvent();

            var buffer = _Peer.Receive();
            if (buffer.Length > 0)
            {
                string something = Encoding.Default.GetString(buffer);
                _Viewer.WriteLine("receive message : " + something);
            }
        }

        public void Send(string message)
        {
            _Peer.Send(Encoding.Default.GetBytes(message));
        }
    }
}
