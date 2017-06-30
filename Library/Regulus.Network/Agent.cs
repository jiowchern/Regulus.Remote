using System;
using System.Net;
using Regulus.Framework;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    public class Agent : IUpdatable<Timestamp>
    {
        private readonly IRecevieable _Recevieable;

        
        private readonly Regulus.Utility.StageMachine<Timestamp> _Machine;
        private readonly SendHandler _SendHandler;
        private readonly ReceiveHandler _ReceiveHandler;
        private IPeer _Peer;

        public Agent(IRecevieable recevieable, ISendable sendable)
        {
            var serializer = Line.CreateSerializer();
            _SendHandler = new SendHandler(sendable  , serializer);
            _ReceiveHandler = new ReceiveHandler(serializer);
            
            _Recevieable = recevieable;        
            _Machine = new StageMachine<Timestamp>();
            
        }

        bool IUpdatable<Timestamp>.Update(Timestamp ticks)
        {            
            _Machine.Update(ticks);
            return true;
        }

        void IBootable.Launch()
        {
            _Recevieable.ReceivedEvent += _ReceiveHandler.Push;

            
        }

        void IBootable.Shutdown()
        {
            _Recevieable.ReceivedEvent -= _ReceiveHandler.Push;
            _Machine.Termination();
        }

        public void Connect(EndPoint end_point)
        {
            _ToConnect(end_point);
        }

        private void _ToConnect(EndPoint end_point)
        {

            var stage = new AgentConnectStage(end_point, _SendHandler , _ReceiveHandler );
            stage.ConnectResultEvent += ConnectResultEvent;
            stage.SuccessEvent += _ToTransmitter;
            stage.FailedEvent += _ToIdle;
            _Machine.Push(stage);
        }

        

        private void _ToTransmitter(EndPoint end_point)
        {
            throw new NotImplementedException();
        }

        void _ToIdle()
        {
            _Peer = null;
            _Machine.Termination();
        }

        public event Action<bool> ConnectResultEvent;

        public void Send(byte[] send_buffer)
        {
            if(_Peer != null)
                _Peer.Send(send_buffer);
        }

        public event Action<byte[]> ReceivedEvent
        {
            add
            {
                if (_Peer != null)
                    _Peer.ReceivedEvent += value;
            }

            remove
            {
                if (_Peer != null)
                    _Peer.ReceivedEvent -= value;
            }
        }
    }
}