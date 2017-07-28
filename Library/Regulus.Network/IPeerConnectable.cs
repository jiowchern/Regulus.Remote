using System;
using System.Net;

namespace Regulus.Network
{
    public interface IPeerConnectable : IPeer
    {
        void Connect(EndPoint endpoint, Action<bool> result);
    }
}