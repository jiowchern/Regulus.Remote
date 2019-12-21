using System;
using System.Net;
using System.Net.Sockets;
using Regulus.Utility;

namespace Regulus.Network.Tcp
{
    public class Listener : IListenable
    {
        private System.Net.Sockets.Socket m_Socket;
        private event Action<IPeer> Acctpe;

        event Action<IPeer> IListenable.AcceptEvent
        {
            add { Acctpe += value; }
            remove { Acctpe -= value; } 
        }

        void IListenable.Bind(int Port)
        {
            m_Socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_Socket.NoDelay = true;

            
            m_Socket.Bind(new IPEndPoint(IPAddress.Any, Port));
            m_Socket.Listen(backlog: 5);
            m_Socket.BeginAccept(Accept, state: null);
        }

        private void Accept(IAsyncResult Ar)
        {
            try
            {
                var socket = m_Socket.EndAccept(Ar);
                lock(Acctpe)
                {
                    Acctpe(new Peer(socket));
                }

                m_Socket.BeginAccept(Accept, state: null);
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

        void IListenable.Close()
        {
            if (m_Socket.Connected)
                m_Socket.Shutdown(SocketShutdown.Both);

            m_Socket.Close();
        }
    }
}