using System;
using System.Collections.Generic;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    public class PeerListener : IStage<Timestamp>
    {
        private readonly ILine _Line;
        private readonly Serializer _Serializer;
        public event Action DoneEvent;

        private readonly Regulus.Utility.StageMachine<Timestamp> _Machine;
        
        public PeerListener(ILine line)
        {
            _Line = line;
            _Serializer = Peer.CreateSerialier();
            _Machine = new StageMachine<Timestamp>();
        }
        void IStage<Timestamp>.Enter()
        {
            _Machine.Push(new SimpleStage<Timestamp>(_Empty , _Empty ,  _ListenRequestUpdate));
        }

        private void _ListenRequestUpdate(Timestamp time)
        {
            /*
             * todo
             * var buffer = _Line.Read();

            if (buffer.Length > 0)
            {
                PeerPackage pkg;
                if (_Serializer.TryBufferToObject(buffer, out pkg) && pkg.Step == PEER_COMMAND.CLIENTTOSERVER_VISIT)
                {
                    var responsePkg = new PeerPackage();
                    responsePkg.Step = PEER_COMMAND.SERVERTOCLIENT_AGREE;
                    _Line.Write(_Serializer.ObjectToBuffer(responsePkg));
                    _Machine.Push(new SimpleStage<Timestamp>(_Empty, _Empty, _ListenAckUpdate));
                }                
            }*/
        }

        private void _ListenAckUpdate(Timestamp obj)
        {
            /*
             * todo
             * var buffer = _Line.Read();

            if (buffer.Length > 0)
            {
                PeerPackage pkg;
                if (_Serializer.TryBufferToObject(buffer, out pkg) && pkg.Step == PEER_COMMAND.CLIENTTOSERVER_ACK)
                {
                    DoneEvent();
                }                
            }*/
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