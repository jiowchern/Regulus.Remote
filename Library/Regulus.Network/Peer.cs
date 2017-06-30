using System;
using System.Net;
using Regulus.Framework;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    internal class Peer : IUpdatable<Timestamp>, IPeer
    {
        private readonly IStream _Stream;

        private readonly Regulus.Utility.StageMachine<Timestamp> _Machine;        

        public Peer(IStream stream , PeerListener listener) : this()
        {
            _Stream = stream;
            listener.DoneEvent += _ToTransmission;
            _Machine.Push(listener);
        }

        private void _ToTransmission(IStream stream)
        {
            var stage = new PeerTransmitter(stream);
            _Machine.Push(stage);
        }

        private Peer()
        {            
            _Machine = new StageMachine<Timestamp>();
        }

        
        bool IUpdatable<Timestamp>.Update(Timestamp arg)
        {
            _Machine.Update(arg);
            return true;
        }

        void IBootable.Launch()
        {
            
        }

        

        void IBootable.Shutdown()
        {
            _Machine.Termination();
        }


        public static Serializer CreateSerialier()
        {
            var builder = new DescriberBuilder(typeof(byte), typeof(byte[]),
                typeof(ListenAgreePackage) , 
                typeof(ConnectRequestPackage) , 
                typeof(ConnectedAckPackage) , 
                typeof(DataPackage));            
            return new Regulus.Serialization.Serializer(builder.Describers);
        }

        EndPoint IPeer.EndPoint
        {
            get { return _Stream.EndPoint; }
        }

        void IPeer.Send(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        event Action<byte[]> IPeer.ReceivedEvent
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event Action IPeer.TimeoutEvent
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }
    }
}