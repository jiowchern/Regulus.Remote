using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Regulus.Network.Package;

namespace Regulus.Network
{
    public class SocketRecevier 
    {
        private readonly System.Net.Sockets.Socket m_Socket;
        
        private readonly List<SocketMessage> m_ReceivePackages;

        private readonly SocketMessage[] m_Empty;

        private SocketMessage m_Message;
        private readonly SocketMessageFactory m_Spawner;
        private EndPoint m_ReceiveEndPoint;
        public SocketRecevier(System.Net.Sockets.Socket Socket, SocketMessageFactory factory )
        {
            m_Empty = new SocketMessage[0];
            m_Socket = Socket;
            
            m_ReceivePackages = new List<SocketMessage>();
            m_Spawner = factory;

            m_ReceiveEndPoint = new IPEndPoint(IPAddress.Any, port: 0);
            
        }

        public void Start()
        {
            Begin();
        }

        private void Begin()
        {
            m_Message = m_Spawner.Spawn();
            
            try
            {
                m_Socket.BeginReceiveFrom(m_Message.Package, offset: 0, size: m_Message.Package.Length, socketFlags: SocketFlags.None, remoteEP: ref m_ReceiveEndPoint, callback: End, state: null);
            }
            catch (SocketException e)
            {
                var error = SocketError.Success;
                error = e.SocketErrorCode;
                m_Message.SetError(error);
                m_Message.SetEndPoint(m_ReceiveEndPoint);
                lock (m_ReceivePackages)
                {
                    m_ReceivePackages.Add(m_Message);
                }
                Begin();
            }
        }

        private void End(IAsyncResult Ar)
        {
            var error = SocketError.Success;
            try
            {
                m_Socket.EndReceiveFrom(Ar, ref m_ReceiveEndPoint);
            }
            catch (SocketException e)
            {
                error = e.SocketErrorCode;
            }
            catch (ObjectDisposedException ode)
            {
                return;
            }
            
            m_Message.SetError(error);
            m_Message.SetEndPoint(m_ReceiveEndPoint);
            lock (m_ReceivePackages)
            {
                m_ReceivePackages.Add(m_Message);
            }
            Begin();
        }


        public SocketMessage[] Received()
        {
            
            lock (m_ReceivePackages)
            {
                var pkgs = m_ReceivePackages.ToArray();                
                m_ReceivePackages.Clear();
                return pkgs;
            }
            
        }

        
    }
}