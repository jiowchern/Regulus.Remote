using System.Net;
using System.Threading.Tasks;

namespace Regulus.Network
{
    public interface IConnectable : IStreamable
    {
        System.Threading.Tasks.Task<bool> Connect(EndPoint endpoint);
        Task Disconnect();
    }
}