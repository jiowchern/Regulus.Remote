using System;
using System.Net.Sockets;
using Regulus.Utility;
using Console = Regulus.Utility.Console;

namespace Regulus.Network.Tests.TestTool
{
    internal class ReceiveContinuousNumberStage : IStage
    {
        private readonly int _Id;
        private readonly IPeer _Peer;
        private readonly Console.IViewer _Viewer;
        private readonly byte[] _Buffer;
        private byte _Data;
        public ReceiveContinuousNumberStage(int id, IPeer peer, Console.IViewer viewer)
        {
            _Id = id;
            _Peer = peer;
            _Viewer = viewer;
            _Buffer = new byte[Config.Default.PackageSize];
        }

        void IStage.Enter()
        {
            _Peer.Receive(_Buffer, 0, _Buffer.Length , _Readed);
        }

        private void _Readed(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (_Buffer[i] != _Data++)
                {
                    throw new SystemException("number receive error!");
                }
            }           
            _Viewer.WriteLine(string.Format("receive count {0}" , count));
        }

        void IStage.Leave()
        {
            
        }

        void IStage.Update()
        {
            
        }
    }
}