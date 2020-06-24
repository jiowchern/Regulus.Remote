using System;
using Regulus.Utility;

namespace Regulus.Network
{
    internal class PeerDisconnecter : IStatus<Timestamp>
    {
        private readonly Line m_Line;        
        public event Action DoneEvent; 

        public PeerDisconnecter(Line Line)
        {
            
            m_Line = Line;
        }

        void IStatus<Timestamp>.Enter()
        {
            
        }

        void IStatus<Timestamp>.Leave()
        {
            
        }

        void IStatus<Timestamp>.Update(Timestamp Obj)
        {
            if(m_Line.WaitSendCount == 0)
                DoneEvent();
        }
    }
}