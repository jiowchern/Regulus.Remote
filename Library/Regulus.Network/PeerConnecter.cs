using System;
using Regulus.Network.Package;
using Regulus.Utility;

namespace Regulus.Network
{
    public class PeerConnecter : IStage<Timestamp>
    {
        
        
        private readonly Line m_Line;
        
        public event Action DoneEvent;
        public event Action TimeoutEvent;
        private long m_TimeoutCount;
        public PeerConnecter(Line Line)
        {            
            m_Line = Line;
        }

        void IStage<Timestamp>.Enter()
        {                        
            m_Line.WriteOperation(PeerOperation.ClienttoserverHello1 );
        }

        void IStage<Timestamp>.Leave()
        {
            
        }

        void IStage<Timestamp>.Update(Timestamp Timestamp)
        {
            m_TimeoutCount += Timestamp.DeltaTicks;
            if (m_TimeoutCount > Timestamp.OneSecondTicks * Config.Timeout)
            {
                TimeoutEvent();
                return;
            }

            var pkg = m_Line.Read();
            if (pkg != null)
            {
                var operation = (PeerOperation)pkg.GetOperation();
                if (operation == PeerOperation.ServertoclientHello1)
                {
                    m_Line.WriteOperation(PeerOperation.ClienttoserverHello2);
                    DoneEvent();
                }
            }
            
        }
    }
}