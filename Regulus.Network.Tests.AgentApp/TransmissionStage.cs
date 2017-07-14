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
            _Command.RegisterLambda(this, (ins) => ins.Disconnect());
        }

        private void Disconnect()
        {
            _Peer.Disconnect();
        }

        void IStage.Leave()
        {
            _Command.Unregister("Send");
            _Command.Unregister("Disconnect");
        }

        void IStage.Update()
        {
            if (_Peer.Status == PEER_STATUS.CLOSE)
                DisconnectEvent();

            var stream = _Peer.Receive();
            if (stream.Length > 0)
            {
                var buffer = new byte[stream.Length];
                stream.Read(0, buffer, 0, stream.Length);
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
