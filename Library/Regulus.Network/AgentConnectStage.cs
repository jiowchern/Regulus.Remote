using System;
using System.Net;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    internal class AgentConnectStage : IStage<Timestamp>
    {
        private readonly EndPoint _EndPoint;
        private readonly SendHandler _SendHandler;
        private readonly ReceiveHandler _ReceiveHandler;
        private long _TimeoutCount;

        public AgentConnectStage(EndPoint end_point, SendHandler send_handler, ReceiveHandler receive_handler)
        {
            _EndPoint = end_point;
            _SendHandler = send_handler;
            _ReceiveHandler = receive_handler;        
        }

        void IStage<Timestamp>.Enter()
        {
            _ReceiveHandler.PopListenAgreeEvent += _ListenAgreeHandler;
            

            _SendHandler.PushConnectRequest(_EndPoint);
        }

        void IStage<Timestamp>.Leave()
        {
            
            _ReceiveHandler.PopListenAgreeEvent -= _ListenAgreeHandler;
        }

        private void _ListenAgreeHandler(ListenAgreePackage arg1, EndPoint end_point)
        {
            if (end_point == _EndPoint)
            {

                _SendHandler.PushConnectedAck(end_point);

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
            _ReceiveHandler.Pop();
        }


        public event Action<bool> ConnectResultEvent;
        public event Action<EndPoint> SuccessEvent;
        public event Action FailedEvent;
    }
}