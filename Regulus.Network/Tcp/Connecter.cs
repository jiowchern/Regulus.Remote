using Regulus.Utility;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Regulus.Network.Tcp
{
    public class Connecter : Peer, IConnectable
    {

        public Connecter() : base(new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {

        }

        System.Threading.Tasks.Task<bool> IConnectable.Connect(EndPoint endpoint)
        {
            System.Net.Sockets.Socket socket = GetSocket();
            return System.Threading.Tasks.Task<bool>.Factory.FromAsync(
                (handler, obj) => socket.BeginConnect(endpoint, handler, null), _ResultConnect, null);
        }

        Task IConnectable.Disconnect()
        {
            System.Net.Sockets.Socket socket = GetSocket();
            return System.Threading.Tasks.Task<bool>.Factory.FromAsync(
                (handler, obj) => socket.BeginDisconnect(false, handler, null), _ResultDisconnect, null);
            
        }

        private bool _ResultDisconnect(IAsyncResult arg)
        {
            bool result = false;
            try
            {

                GetSocket().EndDisconnect(arg);
                result = true;
            }
            catch (SocketException ex)
            {
                Singleton<Log>.Instance.WriteInfo(ex.ToString());
            }
            catch (ObjectDisposedException ode)
            {
                Singleton<Log>.Instance.WriteInfo(ode.ToString());
            }
            finally
            {

                Singleton<Log>.Instance.WriteInfo(string.Format("disconnect result {0}.", result));

            }
            return result;
        }

        private bool _ResultConnect(IAsyncResult Ar)
        {
            bool result = false;
            try
            {
                
                GetSocket().EndConnect(Ar);
                result = true;
            }
            catch (SocketException ex)
            {
                Singleton<Log>.Instance.WriteInfo(ex.ToString());
            }
            catch (ObjectDisposedException ode)
            {
                Singleton<Log>.Instance.WriteInfo(ode.ToString());
            }
            finally
            {

                Singleton<Log>.Instance.WriteInfo(string.Format("connect result {0}.", result));

            }
            return result;
        }


    }
}