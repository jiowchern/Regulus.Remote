using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Net;
using Regulus.Framework;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    internal class Peer : IUpdatable<Timestamp>, IPeer
    {

        public event Action CloseEvent;
        private readonly ILine _Line;

        private readonly Regulus.Utility.StageMachine<Timestamp> _Machine;
        
        private SegmentStream _Stream;
        private PEER_STATUS _Status;

        
        private readonly List<byte> _Sends;
        private bool _RequireDisconnect;

        private Peer(ILine line)
        {
            _Sends = new List<byte>();
            _Line = line;
            _Machine = new StageMachine<Timestamp>();        
            _Stream = new SegmentStream(new SegmentPackage[0]);
            
        }
        public Peer(ILine line , PeerListener listener) : this(line)
        {
            _Status = PEER_STATUS.CONNECTING;
            listener.DoneEvent += _ToTransmission;
            listener.ErrorEvent += _ToClose;
            _Machine.Push(listener);
        }

        public Peer(ILine line, PeerConnecter connecter) : this(line)
        {
            _Status = PEER_STATUS.CONNECTING;
            connecter.DoneEvent += _ToTransmission;
            _Machine.Push(connecter);
        }

        

        private void _ToTransmission()
        {
            _Status = PEER_STATUS.TRANSMISSION;
            _Machine.Push(new SimpleStage<Timestamp>(_StartTransmission , _EndTransmission , _UpdateTransmission ));
        }

        private void _UpdateTransmission(Timestamp obj)
        {
            if (_Sends.Count > 0)
            {
                _Line.Write(PEER_OPERATION.TRANSMISSION, _Sends.ToArray());
                _Sends.Clear();
            }

            if (_RequireDisconnect)
            {            
                _Line.Write(PEER_OPERATION.REQUEST_DISCONNECT, new byte[0]);
                _Disconnect();
            }
             
            var package = _Line.Read();
            if(package == null)
                return;
            var operation = (PEER_OPERATION)package.GetOperation();
            if (operation == PEER_OPERATION.TRANSMISSION)
            {
                _Stream.Add(package);
            }
            else if(operation == PEER_OPERATION.REQUEST_DISCONNECT)
            {
                _Disconnect();
            }
        }

        private void _EndTransmission()
        {            
            
        }

        

        private void _StartTransmission()
        {            
            
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

        public EndPoint EndPoint
        {
            get { return _Line.EndPoint; }
        }
        EndPoint IPeer.EndPoint
        {
            get { return _Line.EndPoint; }
        }

        void IPeer.Send(byte[] buffer)
        {
            _Sends.AddRange(buffer);
        }

        SegmentStream IPeer.Receive()
        {
            var pop = _Stream;
            _Stream = new SegmentStream();
            return pop;
        }

        PEER_STATUS IPeer.Status { get { return _Status; } }

        public void Disconnect()
        {
            _RequireDisconnect = true;
        }


        private void _Disconnect()
        {
            _Status = PEER_STATUS.DISCONNECT;
            var stage = new PeerDisconnecter(_Line);
            stage.DoneEvent += _ToClose;
            _Machine.Push(stage);
        }

        private void _ToClose()
        {
            CloseEvent();
            _Status = PEER_STATUS.CLOSE;
            _Machine.Empty();
        }

        

        public void Break()
        {
            if(_Status != PEER_STATUS.CLOSE)
                _ToClose();
        }
    }
}