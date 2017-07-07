using System;
using System.CodeDom;
using System.Net;
using Regulus.Framework;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    internal class Peer : IUpdatable<Timestamp>, IPeer
    {
        private readonly ILine _Line;

        private readonly Regulus.Utility.StageMachine<Timestamp> _Machine;
        private Action<byte[]> _SendHandler;
        private Func<byte[]> _ReceiveHandler;
        private readonly EmptyArray<byte> _EmptyArray;
        private PEER_STATUS _Status;
        private Serializer _Serialier;

        private Peer(ILine line)
        {
            _Serialier = Peer.CreateSerialier();
            _Line = line;
            _Machine = new StageMachine<Timestamp>();
            _SendHandler = _EmptySend;
            _EmptyArray = new EmptyArray<byte>();
            _ReceiveHandler = _EmptyReceive;
        }
        public Peer(ILine line , PeerListener listener) : this(line)
        {
            _Status = PEER_STATUS.CONNECTING;
            listener.DoneEvent += _ToTransmission;
            _Machine.Push(listener);
        }

        public Peer(ILine line, PeerConnecter connecter) : this(line)
        {
            _Status = PEER_STATUS.CONNECTING;
            connecter.DoneEvent += _ToTransmission;
            _Machine.Push(connecter);
        }

        private void _EmptySend(byte[] obj)
        {
            
        }

        private void _ToTransmission()
        {
            _Status = PEER_STATUS.TRANSMISSION;
            _Machine.Push(new SimpleStage<Timestamp>(_StartTransmission , _EndTransmission , _UpdateTransmission ));
        }

        private void _UpdateTransmission(Timestamp obj)
        {
            
        }

        private void _EndTransmission()
        {            
            _SendHandler = _EmptySend;
            _ReceiveHandler = _EmptyReceive;            
        }

        private byte[] _EmptyReceive()
        {
            return _EmptyArray;
        }

        private void _StartTransmission()
        {            
            _SendHandler = _TransmissionSend;
            _ReceiveHandler = _TransmissionReceive;
        }

        private byte[] _TransmissionReceive()
        {
            
            // todo var buffers = _Line.Read();
            return null;
        }

        private void _TransmissionSend(byte[] buffer)
        {
            var pkg = new PeerPackage();
            pkg.Step = PEER_COMMAND.TRANSMISSION;
            pkg.Buffer = buffer;
            _Line.Write( _Serialier.ObjectToBuffer(pkg));
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


        

        EndPoint IPeer.EndPoint
        {
            get { return _Line.EndPoint; }
        }

        void IPeer.Send(byte[] buffer)
        {
            _SendHandler(buffer);
        }

        byte[] IPeer.Receive()
        {
            return _ReceiveHandler();
        }

        PEER_STATUS IPeer.Status { get { return _Status; } }


        public void Close()
        {
            _Status = PEER_STATUS.DISCONNECT;
            var stage = new PeerDisconnecter(_Line);
            stage.DoneEvent += _ToClose;
            _Machine.Push(stage);
        }

        private void _ToClose()
        {
            _Status = PEER_STATUS.CLOSE;
            _Machine.Empty();
        }

        public static Serializer CreateSerialier()
        {
            var builder = new DescriberBuilder(typeof(byte), typeof(byte[]), typeof(Int32),
                typeof(PEER_COMMAND),
                typeof(PeerPackage));            
            return new Regulus.Serialization.Serializer(builder.Describers);
        }

        public void Release()
        {
            _ToClose();
        }
    }
}