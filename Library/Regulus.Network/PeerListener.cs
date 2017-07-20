using System;
using System.Collections.Generic;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    public class PeerListener : IStage<Timestamp>
    {
        private readonly Line _Line;
        
        public event Action DoneEvent;
        public event Action ErrorEvent;

        private readonly Regulus.Utility.StageMachine<Timestamp> _Machine;
        private long _Timeout;

        public PeerListener(Line line)
        {
            _Line = line;            
            _Machine = new StageMachine<Timestamp>();
        }
        void IStage<Timestamp>.Enter()
        {
            _Machine.Push(new SimpleStage<Timestamp>(_Empty , _Empty ,  _ListenRequestUpdate));
        }

        private void _ListenRequestUpdate(Timestamp time)
        {

            _Timeout += time.DeltaTicks;

            if (_Timeout > Config.HostListenTimeout)
            {
                ErrorEvent();
                return;
            }

            var package = _Line.Read();
            if (package == null)
                return;
            var operation =(PEER_OPERATION)package.GetOperation();
            if (operation == PEER_OPERATION.CLIENTTOSERVER_HELLO1)
            {
                _Line.WriteOperation(PEER_OPERATION.SERVERTOCLIENT_HELLO1 );
                _Machine.Push(new SimpleStage<Timestamp>(_Empty, _Empty, _ListenAckUpdate));
            }
            

            
        }

        private void _ListenAckUpdate(Timestamp time)
        {
            _Timeout += time.DeltaTicks;

            if (_Timeout > Config.HostListenTimeout)
            {
                ErrorEvent();
                return;
            }

            var package = _Line.Read();
            if(package == null)
                return;

            var operation = (PEER_OPERATION)package.GetOperation();
            if (operation == PEER_OPERATION.CLIENTTOSERVER_HELLO2)
            {
                DoneEvent();
            }
            
        }

        private void _Empty()
        {
            
        }

        void IStage<Timestamp>.Leave()
        {
            _Machine.Termination();
        }

        void IStage<Timestamp>.Update(Timestamp obj)
        {
            _Machine.Update(obj);

            
        }
    }
}