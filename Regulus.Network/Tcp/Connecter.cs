using System;
using System.Net;
using System.Net.Sockets;
using Regulus.Utility;

namespace Regulus.Network.Tcp
{
    public class Connecter : Peer , IConnectable
    {
        
        public Connecter() : base(new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {            
            
        }

        System.Threading.Tasks.Task<bool> IConnectable.Connect(EndPoint endpoint)
        {
            var socket = GetSocket();
            return System.Threading.Tasks.Task<bool>.Factory.FromAsync(
                (handler, obj) => socket.BeginConnect(endpoint, handler, null), _Result, null);        
        }

        private bool _Result(IAsyncResult Ar)
        {
            var result = false;
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