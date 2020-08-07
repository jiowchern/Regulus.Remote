using Regulus.Utility;
using System;
using System.Net;
using System.Net.Sockets;

namespace Regulus.Network.Tcp
{
    public class WebListener : IListenable
    {
        private readonly System.Net.Sockets.Socket _Socket;
        private event Action<IPeer> _AcceptEvent;

        public WebListener()
        {
            _Socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _Socket.NoDelay = true;
        }


        event Action<IPeer> IListenable.AcceptEvent
        {
            add { _AcceptEvent += value; }
            remove { _AcceptEvent -= value; }
        }

        void IListenable.Bind(int port)
        {
            _Socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _Socket.Listen(backlog: 5);
            _Socket.BeginAccept(_Accept, state: null);
        }


        private void _Accept(IAsyncResult Ar)
        {
            try
            {

                lock (_AcceptEvent)
                {

                }

                _Socket.BeginAccept(_Accept, state: null);
            }


            catch (SocketException se)
            {
                Singleton<Log>.Instance.WriteInfo(se.ToString());
            }
            catch (ObjectDisposedException ode)
            {
                Singleton<Log>.Instance.WriteInfo(ode.ToString());
            }
            catch (InvalidOperationException ioe)
            {
                Singleton<Log>.Instance.WriteInfo(ioe.ToString());
            }
            catch (Exception e)
            {
                Singleton<Log>.Instance.WriteInfo(e.ToString());
            }
        }

        private async void _Handshaker(System.Net.Sockets.Socket socket)
        {

        }

        void IListenable.Close()
        {
            if (_Socket.Connected)
                _Socket.Shutdown(SocketShutdown.Both);

            _Socket.Close();
        }
    }
}