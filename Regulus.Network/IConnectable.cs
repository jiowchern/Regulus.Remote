using System;
using System.Net;

namespace Regulus.Network
{
    public interface IConnectable : IPeer
    {
        System.Threading.Tasks.Task<bool> Connect(EndPoint Endpoint);
    }
}