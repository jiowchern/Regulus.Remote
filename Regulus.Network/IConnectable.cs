using System;
using System.Net;

namespace Regulus.Network
{
    public interface IConnectable : IPeer
    {
        void Connect(EndPoint Endpoint, Action<bool> Result);
    }
}