using System.Net;
using System.Net.Sockets;

namespace Regulus.Network.Tcp
{
    public static class Tools
    {
        public static int GetAvailablePort()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}