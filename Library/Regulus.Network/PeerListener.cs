using System;
using Regulus.Network.Package;
using Regulus.Utility;

namespace Regulus.Network
{
    public class PeerListener : IStage<Timestamp>
    {
        private readonly Line m_Line;
        
        public event Action DoneEvent;
        public event Action ErrorEvent;

        private readonly StageMachine<Timestamp> m_Machine;
        private long m_Timeout;

        public PeerListener(Line Line)
        {
            m_Line = Line;            
            m_Machine = new StageMachine<Timestamp>();
        }
        void IStage<Timestamp>.Enter()
        {
            m_Machine.Push(new SimpleStage<Timestamp>(Empty , Empty ,  ListenRequestUpdate));
        }

        private void ListenRequestUpdate(Timestamp Time)
        {

            m_Timeout += Time.DeltaTicks;

            if (m_Timeout > Timestamp.OneSecondTicks *Config.Timeout)
            {
                ErrorEvent();
                return;
            }

            var package = m_Line.Read();
            if (package == null)
                return;
            var operation =(PeerOperation)package.GetOperation();
            if (operation == PeerOperation.ClienttoserverHello1)
            {
                m_Line.WriteOperation(PeerOperation.ServertoclientHello1 );
                m_Machine.Push(new SimpleStage<Timestamp>(Empty, Empty, ListenAckUpdate));
            }
            

            
        }

        private void ListenAckUpdate(Timestamp Time)
        {
            m_Timeout += Time.DeltaTicks;

            if (m_Timeout > Timestamp.OneSecondTicks * Config.Timeout)
            {
                ErrorEvent();
                return;
            }

            var package = m_Line.Read();
            if(package == null)
                return;

            var operation = (PeerOperation)package.GetOperation();
            if (operation == PeerOperation.ClienttoserverHello2)
                DoneEvent();
        }

        private void Empty()
        {
            
        }

        void IStage<Timestamp>.Leave()
        {
            m_Machine.Termination();
        }

        void IStage<Timestamp>.Update(Timestamp Obj)
        {
            m_Machine.Update(Obj);

            
        }
    }
}