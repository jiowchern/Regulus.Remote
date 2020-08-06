
using System;
using System.Net;
using System.Net.Sockets;

namespace Regulus.Network
{
    public interface IPeer
    {
        
        bool Connected { get;  }
        System.Threading.Tasks.Task<int> Receive(byte[] buffer, int offset, int count );
        System.Threading.Tasks.Task<int> Send(byte[] buffer, int offset, int count);
        void Close();
    }
}   