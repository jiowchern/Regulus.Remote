using System;
using System.Net;

namespace Regulus.Network
{
    public interface ISocketConnectable : ISocket
    {
        void Connect(EndPoint endpoint, Action<bool> result);
    }
}