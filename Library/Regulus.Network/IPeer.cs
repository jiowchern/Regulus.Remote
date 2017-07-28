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
        void Receive(byte[] readed_byte, int offset, int count,  Action<int ,SocketError> readed);
        void Send(byte[] buffer, int offset_i, int buffer_length, Action<int,SocketError> write_completion);
        void Close();
    }
}