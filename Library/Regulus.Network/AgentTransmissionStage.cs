using System;
using System.Net;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    internal class AgentTransmissionStage : IStage<Timestamp>
    {
        private readonly EndPoint _EndPoint;
        private readonly ReceiveHandler _ReceiveHandler;
        private readonly Transmitter _Transmitter;
        private readonly Updater<Timestamp> _Updater;

        public IPeer Peer { get { return _Transmitter; } }

        public event Action DisconnectEvent
        {
            add { _Transmitter.TimeoutEvent += value; }
            remove { _Transmitter.TimeoutEvent -= value; }
        }

        public AgentTransmissionStage(EndPoint end_point,SendHandler send_handler,ReceiveHandler receive_handler)
        {
            _EndPoint = end_point;
            _ReceiveHandler = receive_handler;
            _Transmitter = new Transmitter(end_point , send_handler , new BufferDispenser(Transmitter.PackageSize));
            _Updater = new Updater<Timestamp>();
        }

        void IStage<Timestamp>.Enter()
        {
            _ReceiveHandler.PopAckEvent += _Receive;
            _ReceiveHandler.PopMessageEvent += _Receive;
            
            _Updater.Add(_Transmitter);
        }

        void IStage<Timestamp>.Leave()
        {
            _Updater.Shutdown();

            
            _ReceiveHandler.PopAckEvent -= _Receive;
            _ReceiveHandler.PopMessageEvent -= _Receive;

            ReleaseEvent();
        }

     

        private void _Receive(MessagePackage arg1, EndPoint arg2)
        {
            if (_EndPoint == arg2)
            {
                _Transmitter.Receive(ref arg1);
            }
        }

        private void _Receive(AckPackage arg1, EndPoint arg2)
        {
            if (_EndPoint == arg2)
            {
                _Transmitter.Receive(ref arg1);
            }
        }

        void IStage<Timestamp>.Update(Timestamp time)
        {
            _Updater.Working(time);
            _ReceiveHandler.Pop();
        }

        public event Action ReleaseEvent;
    }
}