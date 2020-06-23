using Regulus.Network;
using System;
using System.Collections.Generic;
using System.Net;

namespace Regulus.Remote.Tests
{
    internal class SocketHeadReaderTestPeer : Network.IPeer
    {
        private readonly Queue<byte> _Buffer;

        public SocketHeadReaderTestPeer()
        {
            _Buffer = new System.Collections.Generic.Queue<byte>(new byte[] { 0x85 , 0x05 });
        }

        EndPoint IPeer.RemoteEndPoint => throw new NotImplementedException();

        EndPoint IPeer.LocalEndPoint => throw new NotImplementedException();

        bool IPeer.Connected => throw new NotImplementedException();

        void IPeer.Close()
        {
            throw new NotImplementedException();
        }

        System.Threading.Tasks.Task<int> IPeer.Receive(byte[] buffer, int offset, int count )
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                buffer[offset] = _Buffer.Dequeue();
                return 1;
            });
            
            
        }

        System.Threading.Tasks.Task<int> IPeer.Send(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}