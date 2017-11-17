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
        void Receive(byte[] ReadedByte, int Offset, int Count,  Action<int ,SocketError> Readed);
        void Send(byte[] Buffer, int OffsetI, int BufferLength, Action<int,SocketError> WriteCompletion);
        void Close();
    }
}