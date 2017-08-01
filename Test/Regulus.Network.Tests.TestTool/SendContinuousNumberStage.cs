using System;
using System.Net.Sockets;
using Regulus.Utility;
using Random = System.Random;

namespace Regulus.Network.Tests.TestTool
{
    internal class SendContinuousNumberStage : IStage
    {
        private readonly TimeCounter _Counter;
        private readonly float _TimeToSend;
        private readonly int _PackageSize;
        private byte _Data;
        private readonly IPeer _Peer;
        private Random _Random;

        public SendContinuousNumberStage(int fps , int size , IPeer peer)
        {
            _Random = new System.Random(0);
            _Peer = peer;
            _Counter = new TimeCounter();
            _TimeToSend = 1f / fps;
            _PackageSize = size;
        }
        void IStage.Enter()
        {
            _Data = 0;
        }

        void IStage.Leave()
        {
            
        }

        void IStage.Update()
        {
            if (_Counter.Second > _TimeToSend)
            {
                _Counter.Reset();

                var size = _Random.Next(_PackageSize);
                _Peer.Send(_CreateData(size), 0, size, _SendResult);
            }
        }

        private byte[] _CreateData(int package_size)
        {
            var buffer = new byte[package_size];
            for (int i = 0; i < package_size; i++)
            {
                buffer[i] = _Data++;
            }

            return buffer;
        }
        private void _SendResult(int arg1, SocketError arg2)
        {
            
        }
    }
}