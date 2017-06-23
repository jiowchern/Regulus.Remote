using System;
using System.Net;
using Regulus.Framework;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    internal class Listener : IUpdatable<Timestamp>
    {
        public readonly EndPoint EndPoint;
        private readonly SendHandler _SendHandler;
        private readonly long _Timeout;

        public event Action SuccessEvent;
        public event Action TimeoutEvent;
        private long _TimeoutCount;
        
        public Listener(EndPoint end_point, SendHandler send_handler , long timeout)
        {
        
            EndPoint = end_point;
            _SendHandler = send_handler;
            _Timeout = timeout;
        }

        

        void IBootable.Launch()
        {
            _SendHandler.PushListenAgree(EndPoint);            
        }

        void IBootable.Shutdown()
        {
            
        }

        bool IUpdatable<Timestamp>.Update(Timestamp ticks)
        {
            _TimeoutCount += ticks.DeltaTicks;
            var timeout = _TimeoutCount > _Timeout;
            if (timeout)
                TimeoutEvent();
            return timeout == false;
        }

        public void SetConnectAck(ConnectedAckPackage package)
        {
            SuccessEvent();
        }
        
    }
}