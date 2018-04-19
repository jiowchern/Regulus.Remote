using System;
using System.Net;
using System.Net.Sockets;

namespace Regulus.Network
{
    public interface IPeer
    {
        EndPoint RemoteEndPoint { get;  }
        EndPoint LocalEndPoint { get;  }
        bool Connected { get;  }
        void Receive(byte[] buffer, int offset, int count , Action<int> done);
        Task Send(byte[] buffer, int offset, int count);
        void Close();
    }
}   