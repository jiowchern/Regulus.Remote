using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Regulus.Framework;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    internal class Peer : IUpdatable<Timestamp>, IPeer
    {

        public event Action CloseEvent;
        private readonly Line _Line;

        private readonly Regulus.Utility.StageMachine<Timestamp> _Machine;
        
        private readonly SegmentStream _Stream;
        private PEER_STATUS _Status;

        class ReadRequest
        {
            public byte[] Buffer;
            public int Offset;
            public int Count;
            public Action<int, SocketError> ReadDoneHandler;
        }
        private readonly List<byte> _Sends;
        private readonly Queue<ReadRequest> _ReadRequests;
        private bool _RequireDisconnect;

        private Peer(Line line)
        {
            _ReadRequests = new Queue<ReadRequest>();
            _Sends = new List<byte>();
            _Line = line;
            _Machine = new StageMachine<Timestamp>();        
            _Stream = new SegmentStream(new SocketMessage[0]);
            
        }
        public Peer(Line line , PeerListener listener) : this(line)
        {
            _Status = PEER_STATUS.CONNECTING;
            listener.DoneEvent += _ToTransmission;
            listener.ErrorEvent += _ToClose;
            _Machine.Push(listener);
        }

        public Peer(Line line, PeerConnecter connecter) : this(line)
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
            lock (_Sends)
            {
                if (_Sends.Count > 0)
                {
                    _Line.WriteTransmission(_Sends.ToArray());
                    _Sends.Clear();
                }
            }
            

            while (_Stream.Count > 0 && _ReadRequests.Count > 0)
            {
                ReadRequest request = null;

                lock (_ReadRequests)
                {
                    request = _ReadRequests.Dequeue();                    
                }

                var readCount = _Stream.Read(request.Buffer, request.Offset, request.Count);
                if (readCount > 0)
                {
                    request.ReadDoneHandler(readCount, SocketError.Success);

                }

            }
            

            if (_RequireDisconnect)
            {            
                _Line.WriteOperation(PEER_OPERATION.REQUEST_DISCONNECT);
                _Disconnect();
            }

            SocketMessage message = null;
            while ((message = _Line.Read()) != null)
            {
                var package = message;
                
                var operation = (PEER_OPERATION)package.GetOperation();
                if (operation == PEER_OPERATION.TRANSMISSION)
                {
                    _Stream.Add(package);
                }
                else if (operation == PEER_OPERATION.REQUEST_DISCONNECT)
                {
                    _Disconnect();
                }
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

        void IPeer.Send(byte[] buffer, int offset, int count, Action<int, SocketError> write_completion)
        {
            int len = count < buffer.Length ? count : buffer.Length;

            lock (_Sends)
            {
                for (int i = offset; i < len; i++)
                {
                    _Sends.Add(buffer[i]);
                }
            }
            
            write_completion(len , SocketError.Success);
        }

        void IPeer.Receive(byte[] buffer, int offset, int count, Action<int, SocketError> read_completion)
        {

            var requests = new ReadRequest();
            requests.Buffer = buffer;
            requests.Count = count;
            requests.Offset = offset;
            requests.ReadDoneHandler = read_completion;
            lock (_ReadRequests)
            {
                _ReadRequests.Enqueue(requests);
            }
            
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