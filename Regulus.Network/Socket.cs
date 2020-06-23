using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using Regulus.Framework;
using Regulus.Network.Package;
using Regulus.Utility;

namespace Regulus.Network
{
    public class Socket : IUpdatable<Timestamp>
    {

        public event Action CloseEvent;
        private readonly Line _Line;

        private readonly StatusMachine<Timestamp> m_Machine;

        
        private PeerStatus _Status;

        
        private readonly System.Collections.Concurrent.ConcurrentQueue<SendTask> _SendTasks;
        private readonly SegmentStream _Stream;
        public Action<int, SocketError> WriteDoneHandler;

        



        private Socket(Line Line)
        {
            _Stream = new SegmentStream(new SocketMessage[0]);
            _SendTasks = new ConcurrentQueue<SendTask>();
        
            
            _Line = Line;
            m_Machine = new StatusMachine<Timestamp>();
            

        }
        public Socket(Line Line, PeerListener Listener) : this(Line)
        {
            _Status = PeerStatus.Connecting;
            Listener.DoneEvent += ToTransmission;
            Listener.ErrorEvent += ToClose;
            m_Machine.Push(Listener);
        }

        public Socket(Line Line, PeerConnecter Connecter) : this(Line)
        {
            _Status = PeerStatus.Connecting;
            Connecter.DoneEvent += ToTransmission;
            m_Machine.Push(Connecter);
        }



        private void ToTransmission()
        {
            _Status = PeerStatus.Transmission;
            var stage = new PeerTransmission(_SendTasks, _Line, _Stream);
            stage.DisconnectEvent += _Disconnect;
            m_Machine.Push(stage);            
        }

       




        bool IUpdatable<Timestamp>.Update(Timestamp Arg)
        {
            m_Machine.Update(Arg);
            return true;
        }

        void IBootable.Launch()
        {

        }



        void IBootable.Shutdown()
        {
            m_Machine.Termination();
        }




        public EndPoint EndPoint {get{return _Line.EndPoint; } } 

        public SendTask Send(byte[] buffer, int offset, int count)
        {
            var task = new SendTask() {Buffer = buffer , Offset =  offset , Count = count };
            _SendTasks.Enqueue(task);
            return task;

        }



        public System.Threading.Tasks.Task<int> Receive(byte[] buffer, int offset, int count)
        {

            return System.Threading.Tasks.Task<int>.Run(() => {

                var readCount = _Stream.Read(buffer, offset, count);
                var r = new Regulus.Utility.AutoPowerRegulator(new PowerRegulator());
                while(readCount == 0)
                {
                    r.Operate();
                    readCount = _Stream.Read(buffer, offset, count);
                }
                return readCount;
            });


        }

        public PeerStatus Status {get { return _Status; } }

        public void Disconnect()
        {
            _Disconnect();
        }


        private void _Disconnect()
        {
            if(_Status != PeerStatus.Close)
            {
                _Status = PeerStatus.Disconnect;
                var stage = new PeerDisconnecter(_Line);
                stage.DoneEvent += ToClose;
                m_Machine.Push(stage);
            }
            
        }

        private void ToClose()
        {
            CloseEvent();
            _Status = PeerStatus.Close;
            m_Machine.Empty();
        }



        public void Break()
        {
            if (_Status != PeerStatus.Close)
                ToClose();
        }
    }
}