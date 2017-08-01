using System;
using System.Net.Sockets;
using Regulus.Utility;
using Console = Regulus.Utility.Console;

namespace Regulus.Network.Tests.TestTool
{
    internal class ReceiveStringStage : IStage
    {
        private readonly int _Id;
        private readonly IPeer _Peer;
        private readonly Console.IViewer _Viewer;
        private readonly byte[] _Buffer;
        public ReceiveStringStage(int id , IPeer peer, Console.IViewer viewer)
        {
            _Id = id;
            _Peer = peer;
            _Viewer = viewer;
            _Buffer = new byte[Config.PackageSize];
        }

        void IStage.Enter()
        {
            _Peer.Receive(_Buffer, 0, _Buffer.Length, _Readed);
        }

        void IStage.Leave()
        {            
        }

        void IStage.Update()
        {            
        }

        private void _Readed(int read_count, SocketError error)
        {
            
            var message = System.Text.Encoding.Default.GetString(_Buffer, 0, read_count);
            _Viewer.WriteLine(String.Format("transmitter{0} : {1}", _Id, message));


            for (int i = 0; i < _Buffer.Length; i++)
            {
                _Buffer[i] = 0;
            }

            _Peer.Receive(_Buffer, 0, _Buffer.Length, _Readed);
        }
    }
}