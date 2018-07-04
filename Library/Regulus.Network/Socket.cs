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
        private readonly Line m_Line;

        private readonly StageMachine<Timestamp> m_Machine;

        private readonly SegmentStream m_Stream;
        private PeerStatus m_Status;

        Task _ReadTask;
        private readonly System.Collections.Concurrent.ConcurrentQueue<Task> _SendTasks;
        private readonly System.Collections.Generic.List<byte> _SendBytes;



        public Action<int, SocketError> WriteDoneHandler;

        private bool m_RequireDisconnect;



        private Socket(Line Line)
        {
            _SendBytes = new List<byte>();
            _SendTasks = new ConcurrentQueue<Task>();
            _ReadTask = new Task();
            
            m_Line = Line;
            m_Machine = new StageMachine<Timestamp>();
            m_Stream = new SegmentStream(new SocketMessage[0]);

        }
        public Socket(Line Line, PeerListener Listener) : this(Line)
        {
            m_Status = PeerStatus.Connecting;
            Listener.DoneEvent += ToTransmission;
            Listener.ErrorEvent += ToClose;
            m_Machine.Push(Listener);
        }

        public Socket(Line Line, PeerConnecter Connecter) : this(Line)
        {
            m_Status = PeerStatus.Connecting;
            Connecter.DoneEvent += ToTransmission;
            m_Machine.Push(Connecter);
        }



        private void ToTransmission()
        {
            m_Status = PeerStatus.Transmission;
            m_Machine.Push(new SimpleStage<Timestamp>(StartTransmission, EndTransmission, UpdateTransmission));
        }

        private void UpdateTransmission(Timestamp Obj)
        {
            while (_SendTasks.Count > 0)
            {
                Task task = null;
                if (_SendTasks.TryDequeue(out task))
                {
                    for (int i = task.Offset; i < task.Count; i++)
                    {
                        _SendBytes.Add(task.Buffer[i]);
                    }
                    task.Done(task.Count);
                }
            }

            if (_SendBytes.Count > 0)
            {
                m_Line.WriteTransmission(_SendBytes.ToArray());
                _SendBytes.Clear();
            }
                
            while (m_Stream.Count > 0 )
            {
                
                lock (_ReadTask)
                {
                    var handler = _ReadTask;
                    if (handler.Buffer != null)
                    {
                        var readCount = m_Stream.Read(handler.Buffer, handler.Offset, handler.Count);
                        if (readCount > 0)
                            handler.Done(readCount);
                    }
                }
            }
                


            if (m_RequireDisconnect)
            {
                m_Line.WriteOperation(PeerOperation.RequestDisconnect);
                _Disconnect();
            }

            SocketMessage message = null;
            while ((message = m_Line.Read()) != null)
            {
                var package = message;

                var operation = (PeerOperation)package.GetOperation();
                if (operation == PeerOperation.Transmission)
                    m_Stream.Add(package);
                else if (operation == PeerOperation.RequestDisconnect)
                    _Disconnect();
            }


        }

        private void EndTransmission()
        {

        }



        private void StartTransmission()
        {

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




        public EndPoint EndPoint {get{return m_Line.EndPoint; } } 

        public Task Send(byte[] buffer, int offset, int count)
        {
            var task = new Task() {Buffer = buffer , Offset =  offset , Count = count };
            _SendTasks.Enqueue(task);
            return task;

        }



        public void Receive(byte[] buffer, int offset, int count, Action<int> done)
        {
            
            lock (_ReadTask)
            {
                _ReadTask.Buffer = buffer;
                _ReadTask.Offset = offset;
                _ReadTask.Count = count;
                _ReadTask._DoneEvent = done;
            }
            
        }

        public PeerStatus Status {get { return m_Status; } }

        public void Disconnect()
        {
            m_RequireDisconnect = true;
        }


        private void _Disconnect()
        {
            m_Status = PeerStatus.Disconnect;
            var stage = new PeerDisconnecter(m_Line);
            stage.DoneEvent += ToClose;
            m_Machine.Push(stage);
        }

        private void ToClose()
        {
            CloseEvent();
            m_Status = PeerStatus.Close;
            m_Machine.Empty();
        }



        public void Break()
        {
            if (m_Status != PeerStatus.Close)
                ToClose();
        }
    }
}