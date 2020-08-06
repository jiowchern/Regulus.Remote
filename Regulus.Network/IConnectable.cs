using System;
using System.Net;

namespace Regulus.Network
{
    public interface IConnectable : IStreamable
    {
        System.Threading.Tasks.Task<bool> Connect(EndPoint Endpoint);
    }
}