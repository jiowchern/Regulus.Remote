using System;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    public class PeerListener : IStage<Timestamp>
    {
        private readonly IStream _Stream;
        private readonly Serializer _Serializer;
        public event Action<IStream> DoneEvent;

        private readonly Regulus.Utility.StageMachine<Timestamp> _Machine;

        public PeerListener(IStream stream)
        {
            _Stream = stream;
            _Serializer = Peer.CreateSerialier();
            _Machine = new StageMachine<Timestamp>();
        }
        void IStage<Timestamp>.Enter()
        {
            _Machine.Push(new SimpleStage<Timestamp>(_Empty , _Empty ,  _ListenRequestUpdate));
        }

        private void _ListenRequestUpdate(Timestamp time)
        {
            var buffer = _Stream.Read();

            if (buffer.Length > 0)
            {
                ConnectRequestPackage pkg;
                if (_Serializer.TryBufferToObject(buffer, out pkg))
                {
                    _Stream.Write(_Serializer.ObjectToBuffer(new ListenAgreePackage()));
                    _Machine.Push(new SimpleStage<Timestamp>(_Empty, _Empty, _ListenAckUpdate));
                }                
            }
        }

        private void _ListenAckUpdate(Timestamp obj)
        {
            var buffer = _Stream.Read();

            if (buffer.Length > 0)
            {
                ConnectedAckPackage pkg;
                if (_Serializer.TryBufferToObject(buffer, out pkg))
                {
                    DoneEvent(_Stream);
                }                
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