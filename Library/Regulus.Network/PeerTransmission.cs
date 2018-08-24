using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Regulus.Network.Package;
using Regulus.Utility;

namespace Regulus.Network
{
    internal class PeerTransmission : IStage<Timestamp>
    {
        private readonly ConcurrentQueue<Task> _SendTasks;
        private readonly Line _Line;
        private readonly Task _ReadTask;
        private readonly List<byte> _SendBytes;
        private readonly SegmentStream _Stream;
        public event Action DisconnectEvent ;
        private long _Timeout;
        public PeerTransmission(ConcurrentQueue<Task> send_tasks, Line line, Task read_task)
        {
            _SendTasks = send_tasks;
            _Line = line;
            _ReadTask = read_task;
            _SendBytes = new List<byte>();
            _Stream = new SegmentStream(new SocketMessage[0]);
        }

        void IStage<Timestamp>.Enter()
        {
            
        }

        void IStage<Timestamp>.Leave()
        {
            _Line.WriteOperation(PeerOperation.RequestDisconnect);
        }

        void IStage<Timestamp>.Update(Timestamp timestamp)
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
                _Line.WriteTransmission(_SendBytes.ToArray());
                _SendBytes.Clear();
            }

            while (_Stream.Count > 0)
            {

                lock (_ReadTask)
                {
                    var handler = _ReadTask;
                    if (handler.Buffer != null)
                    {
                        var readCount = _Stream.Read(handler.Buffer, handler.Offset, handler.Count);
                        if (readCount > 0)
                            handler.Done(readCount);
                    }
                }
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
            if (_Timeout > Config.Timeout)
            {
                DisconnectEvent();
            }

        }
    }
}