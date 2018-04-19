using System;
using Regulus.Utility;
using Console = Regulus.Utility.Console;

namespace Regulus.Network.Tests.TestTool
{
    internal class SendStringStage : IStage
    {
        private readonly IPeer _Peer;
        private readonly Console.IViewer _Viewer;
        private readonly Command _Command;
        private readonly string _CommandSendString;

        public SendStringStage(int id , IPeer peer, Console.IViewer viewer , Command command)
        {
            _Peer = peer;
            _Viewer = viewer;
            _Command = command;
            _CommandSendString = string.Format("send{0}",id);
        }

        void IStage.Enter()
        {
            _Command.Register<string>(_CommandSendString, Send);
        }

        void IStage.Leave()
        {
            _Command.Unregister(_CommandSendString);
        }

        void IStage.Update()
        {            
        }
        public void Send(string message)
        {
            var buffer = _ToBytes(message);
            _Peer.Send(buffer, 0, buffer.Length ).DoneEvent += (send_count ) =>
            {
                _Viewer.WriteLine(string.Format("send bytes {0} ", send_count ));
            };
        }
        public byte[] _ToBytes(string message)
        {
            return System.Text.Encoding.Default.GetBytes(message);
        }
    }
}