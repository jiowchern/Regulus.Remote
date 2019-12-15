using System.Threading;
using Regulus.Utility;

namespace Regulus.Network.Rudp
{

    public class ConnectProvider :  IConnectProviderable 
    {
        private readonly ISocket m_Socket;
        private readonly ITime m_Time;
        private readonly Agent m_Agent;
        private volatile bool m_Enable;
        
        public ConnectProvider(ISocket socket)
        {        
            m_Socket = socket;
            m_Time = new Time();
            m_Agent = new Agent(m_Socket, m_Socket);            
        }

        private void Run(object State)
        {
            
            var wait = new AutoPowerRegulator(new PowerRegulator(30));
            
            var updater = new Updater<Timestamp>();
            updater.Add(m_Agent);
            while (m_Enable)
            {
        
                m_Time.Sample();
                updater.Working(new Timestamp(m_Time.Now , m_Time.Delta));
                wait.Operate();
            }
            updater.Shutdown();
        }


        public void Launch()
        {
            m_Socket.Bind(Port: 0);
            m_Enable = true;
            ThreadPool.QueueUserWorkItem(Run, state: null);
        }

        public void Shutdown()
        {
            m_Enable = false;
        }

        IConnectable IConnectProviderable.Spawn()
        {
            return new Connector(m_Agent);
        }
    }
}
