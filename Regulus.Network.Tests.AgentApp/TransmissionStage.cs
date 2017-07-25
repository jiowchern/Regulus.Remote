using System;
using System.Net.Sockets;
using System.Text;
using Regulus.Network.RUDP;
using Regulus.Utility;
using Console = Regulus.Utility.Console;

namespace Regulus.Network.Tests.AgentApp
{
    internal class TransmissionStage : IStage
    {
        private readonly ISocket _Peer;
        private readonly Command _Command;
        private readonly Console.IViewer _Viewer;
        public event Action DisconnectEvent;

        private readonly byte[] _Buffer;
        public TransmissionStage(ISocket peer, Command command , Console.IViewer viewer)
        {
            _Peer = peer;
            _Command = command;
            _Viewer = viewer;
            _Buffer = new byte[Config.PackageSize];
        }

        void IStage.Enter()
        {
            _Command.RegisterLambda<TransmissionStage , string>(this, (ins , message) => ins.Send(message) );
            _Command.RegisterLambda(this, (ins) => ins.Disconnect());
            

            _Peer.Receive(_Buffer, 0, _Buffer.Length, _Readed);
        }

        private void Disconnect()
        {
            _Peer.Close();
        }

        void IStage.Leave()
        {
            _Command.Unregister("Send");
            _Command.Unregister("Disconnect");
        }

        void IStage.Update()
        {
            if (_Peer.Connected == false)
                DisconnectEvent();

            
            
            
        }

        private void _Readed(int readed_count, SocketError error)
        {
            string something = Convert.ToBase64String(_Buffer,0,readed_count);
            for (int i = 0; i < _Buffer.Length; i++)
            {
                _Buffer[i] = 0;
            }
            _Viewer.WriteLine("receive message : " + something);

            _Peer.Receive(_Buffer, 0, _Buffer.Length, _Readed);

        }

        public void Send(string message)
        {
            var buffer = Encoding.Default.GetBytes(message);
            _Peer.Send(buffer , 0 , buffer.Length , _Writed);
        }

        private void _Writed(int arg1, SocketError arg2)
        {
            
        }
    }
}
