using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Regulus.Framework;
using Regulus.Network.RUDP;
using Regulus.Utility;
using Console = Regulus.Utility.Console;

namespace Regulus.Network.Tests.TestTool
{
    internal class FakeSocket : ISocket ,IUpdatable
    {
        private static int _InstanceId;
        private readonly int _Id;
        private readonly IPEndPoint _IpEndPoint;
        private readonly Command _Command;
        private readonly Console.IViewer _Viewer;
        private int _MissAmount;
        private int _MissCount;
        private readonly ITime _Time;
        private readonly System.Random _Random;

        private float _SendTime;
        private float _SendTimeJitter;
        
        class Record
        {
            public long TimeUp;
            public SocketMessage Message;
        }

        private readonly List<Record> _SendRecords;
        private readonly List<SocketMessage> _ReceiveMessages;
        public FakeSocket(IPEndPoint ip_end_point, Command command , Console.IViewer viewer )
        {
            _IpEndPoint = ip_end_point;
            _Command = command;
            _Viewer = viewer;
            _Id = ++_InstanceId;
            _MissCount = ushort.MaxValue + 1;
            _Time = new Time();
            _Time.Sample();
            _Random = new System.Random(0);
            _SendRecords = new List<Record>();
            _ReceiveMessages = new List<SocketMessage>();
            _SendTime = 0;
            _SendTimeJitter = 0f;
        }

        public event Action<SocketMessage> SendEvent;
        SocketMessage[] ISocketRecevieable.Received()
        {
            lock (_ReceiveMessages)
            {
                var messages = _ReceiveMessages.ToArray();
                _ReceiveMessages.Clear();
                return messages;
            }
            
        }

        void ISocketSendable.Transport(SocketMessage message)
        {
            
            if (--_MissCount == 0)
            {
                _MissCount = _MissAmount;
                return;
            }
            var seq = message.GetSeq();
            var record = new Record();
            var jitter = _Random.NextDouble() * _SendTimeJitter - _SendTimeJitter / 2f;
            var timeUp = _SendTime + jitter;            
            record.TimeUp = (long)(timeUp * Timestamp.OneSecondTicks);
            record.Message = new SocketMessage(Config.Default.PackageSize);
            for (int i = 0; i < message.Package.Length; i++)
            {
                record.Message.Package[i] = message.Package[i];
            }
            record.Message.SetEndPoint(_IpEndPoint);
            _DoneEvent(message);
            lock (_SendRecords)
            {
                _SendRecords.Add(record);
            }
            

        }

        event Action<SocketMessage> _DoneEvent;
        event Action<SocketMessage> ISocketSendable.DoneEvent
        {
            add { _DoneEvent += value; }
            remove { _DoneEvent -= value; }
        }

        void ISocket.Close()
        {
            _Command.Unregister("miss" + _Id);
            _Command.Unregister("speed" + _Id);
        }

        void ISocket.Bind(int port)
        {
            _IpEndPoint.Port = port;
            _Command.Register<int>("miss" + _Id , _Miss);
            _Command.Register<float,float>("speed" + _Id, _Speed);
        }

        private void _Speed(float speed,float jitter )
        {
            _SendTime = speed;
            _SendTimeJitter = jitter;
        }

        private void _Miss(int num)
        {
            _MissAmount = num;
            _MissCount = _MissAmount;
        }

        void IBootable.Launch()
        {
            
        }

        void IBootable.Shutdown()
        {
            
        }

        bool IUpdatable.Update()
        {
            _Time.Sample();
            lock (_SendRecords)
            {
                foreach (var sendRecord in _SendRecords.ToArray())
                {
                    if ((sendRecord.TimeUp -= _Time.Delta) <= 0)
                    {
                        _SendRecords.RemoveAll((r) => r.Message == sendRecord.Message);
                        SendEvent(sendRecord.Message);
                    }
                }

                
            }
            

            return true;
        }

        public void Receive(SocketMessage obj)
        {
            lock (_ReceiveMessages)
            {
                _ReceiveMessages.Add(obj);
            }
            
        }
    }
}