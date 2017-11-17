using System;
using System.Threading;
using Regulus.Utility;

namespace Regulus.Network.Rudp
{
    public class Server : IServer
    {
        private readonly ISocket m_Socket;
        private readonly ITime m_Time;
        private Host m_Host;
        private volatile bool m_Enable;
        private event Action<IPeer> AcceptEvent;

        public Server(ISocket Socket)
        {
            m_Host = new Host(Socket,Socket);
            m_Socket = Socket;
            m_Time = new Time();
        }

        event Action<IPeer> IServer.AcceptEvent
        {
            add => AcceptEvent += value;
            remove => AcceptEvent -= value;
        }

        void IServer.Bind(int Port)
        {
            m_Socket.Bind(Port);
            m_Host.AcceptEvent += Accept;
            m_Enable = true;
            ThreadPool.QueueUserWorkItem(Run, state: null);
        }

        private void Run(object State)
        {
            var updater = new Updater<Timestamp>();
            updater.Add(m_Host);

            var wait = new AutoPowerRegulator(new PowerRegulator());
            while (m_Enable)
            {
                m_Time.Sample();
                updater.Working(new Timestamp(m_Time.Now, m_Time.Delta));
                wait.Operate();
            }

            updater.Shutdown();

        }

        private void Accept(Regulus.Network.Socket rudp_socket)
        {
            AcceptEvent(new Peer(rudp_socket));
        }

        void IServer.Close()
        {
            m_Socket.Close();
            m_Host.AcceptEvent -= Accept;
            m_Enable = false;
        }
    }
}
