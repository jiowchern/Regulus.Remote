using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Regulus.Network.Package;
using Regulus.Utility;

namespace Regulus.Network
{
    internal class PeerTransmission : IStatus<Timestamp>
    {
        private readonly ConcurrentQueue<SendTask> _SendTasks;
        private readonly Line _Line;        
        private readonly List<byte> _SendBytes;
        private readonly SegmentStream _Stream;
        public event Action DisconnectEvent ;
        private long _Timeout;
        public PeerTransmission(ConcurrentQueue<SendTask> send_tasks, Line line , SegmentStream stream)
        {
            _SendTasks = send_tasks;
            _Line = line;
            
            _SendBytes = new List<byte>();
            _Stream = stream;
        }

        void IStatus<Timestamp>.Enter()
        {
            
        }

        void IStatus<Timestamp>.Leave()
        {
            _Line.WriteOperation(PeerOperation.RequestDisconnect);
        }

        void IStatus<Timestamp>.Update(Timestamp timestamp)
        {



            while (_SendTasks.Count > 0)
            {
                SendTask task = null;
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
                _Line.WriteTransmission(_SendBytes.ToArray());
                _SendBytes.Clear();
            }

            SocketMessage message ;
            while ((message = _Line.Read()) != null)
            {
                _Timeout = 0;
                var package = message;

                var operation = (PeerOperation)package.GetOperation();
                if (operation == PeerOperation.Transmission)
                    _Stream.Add(package);
                else if (operation == PeerOperation.RequestDisconnect)
                    DisconnectEvent();
            }

            _Timeout += timestamp.DeltaTicks;
            if (_Timeout > Config.Timeout * Timestamp.OneSecondTicks)
            {
                DisconnectEvent();
            }

        }
    }
}