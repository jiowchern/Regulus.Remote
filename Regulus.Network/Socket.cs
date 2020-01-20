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

        private readonly StatusMachine<Timestamp> m_Machine;

        
        private PeerStatus m_Status;

        Task _ReadTask;
        private readonly System.Collections.Concurrent.ConcurrentQueue<Task> _SendTasks;
        



        public Action<int, SocketError> WriteDoneHandler;

        



        private Socket(Line Line)
        {
            
            _SendTasks = new ConcurrentQueue<Task>();
            _ReadTask = new Task();
            
            m_Line = Line;
            m_Machine = new StatusMachine<Timestamp>();
            

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
            var stage = new PeerTransmission(_SendTasks, m_Line, _ReadTask);
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
            _Disconnect();
        }


        private void _Disconnect()
        {
            if(m_Status != PeerStatus.Close)
            {
                m_Status = PeerStatus.Disconnect;
                var stage = new PeerDisconnecter(m_Line);
                stage.DoneEvent += ToClose;
                m_Machine.Push(stage);
            }
            
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