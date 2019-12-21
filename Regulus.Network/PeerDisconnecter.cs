using System;
using Regulus.Utility;

namespace Regulus.Network
{
    internal class PeerDisconnecter : IStage<Timestamp>
    {
        private readonly Line m_Line;        
        public event Action DoneEvent; 

        public PeerDisconnecter(Line Line)
        {
            
            m_Line = Line;
        }

        void IStage<Timestamp>.Enter()
        {
            
        }

        void IStage<Timestamp>.Leave()
        {
            
        }

        void IStage<Timestamp>.Update(Timestamp Obj)
        {
            if(m_Line.WaitSendCount == 0)
                DoneEvent();
        }
    }
}