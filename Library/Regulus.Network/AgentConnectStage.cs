using System;
using System.Net;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    internal class AgentConnectStage : IStage<Timestamp>
    {
        private readonly EndPoint _EndPoint;
       
        
        private long _TimeoutCount;

        public AgentConnectStage(EndPoint end_point)
        {
            _EndPoint = end_point;
          
            
        }

        void IStage<Timestamp>.Enter()
        {
            
            

           
        }

        void IStage<Timestamp>.Leave()
        {
            
            
        }

        private void _ListenAgreeHandler(PeerPackage arg1, EndPoint end_point)
        {
            if (end_point == _EndPoint)
            {

               

                ConnectResultEvent?.Invoke(true);
                SuccessEvent(end_point);
            }
        }

        

        void IStage<Timestamp>.Update(Timestamp time)
        {
            _TimeoutCount += time.Ticks;
            if (_TimeoutCount > Config.AgentConnectTimeout)
            {
                FailedEvent();
                ConnectResultEvent?.Invoke(false);                
            }
            
        }


        public event Action<bool> ConnectResultEvent;
        public event Action<EndPoint> SuccessEvent;
        public event Action FailedEvent;
    }
}