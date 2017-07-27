using System;
using System.Net;
using System.Net.Sockets;
using Regulus.Utility;

namespace Regulus.Network
{
    public class TcpServer : ISocketServer
    {
        private Socket _Socket;
        private event Action<ISocket> _AcctpeEvent;

        event Action<ISocket> ISocketServer.AcceptEvent
        {
            add { _AcctpeEvent += value; }
            remove { _AcctpeEvent -= value; }
        }

        void ISocketServer.Bind(int port)
        {
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _Socket.NoDelay = true;

            
            _Socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _Socket.Listen(5);
            _Socket.BeginAccept(_Accept, null);
        }

        private void _Accept(IAsyncResult ar)
        {
            try
            {
                var socket = _Socket.EndAccept(ar);
                lock(_AcctpeEvent)
                {
                    _AcctpeEvent(new TcpSocket(socket));
                }

                _Socket.BeginAccept(_Accept, null);
            }

                
            catch(SocketException se)
            {
                Singleton<Log>.Instance.WriteInfo(se.ToString());
            }
            catch(ObjectDisposedException ode)
            {
                Singleton<Log>.Instance.WriteInfo(ode.ToString());
            }
            catch(InvalidOperationException ioe)
            {
                Singleton<Log>.Instance.WriteInfo(ioe.ToString());
            }
            catch(Exception e)
            {
                Singleton<Log>.Instance.WriteInfo(e.ToString());
            }
        }

        void ISocketServer.Close()
        {
            if (_Socket.Connected)
            {
                _Socket.Shutdown(SocketShutdown.Both);
            }

            _Socket.Close();
        }
    }
}