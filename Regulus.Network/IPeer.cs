
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
        System.Threading.Tasks.Task<int> Receive(byte[] buffer, int offset, int count );
        SendTask Send(byte[] buffer, int offset, int count);
        void Close();
    }
}   