using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Regulus.Extension;
using Regulus.Framework;
using Regulus.Network.Profile;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    public class WiringOperator : IUpdatable<Timestamp>
    {
        private readonly ISocketSendable _SocketSendable;
        private readonly ISocketRecevieable _SocketRecevieable;

        private readonly Dictionary<EndPoint , Line> _Lines;
        private readonly Queue<Line> _Exits;
        private Queue<SocketMessage> _Messages;
        private readonly Logger _Logger;
        public WiringOperator(ISocketSendable socket_sendable , ISocketRecevieable socket_recevieable)
        {
            _Logger = new Logger(1f / 10f);
            _Lines = new Dictionary<EndPoint, Line>();
            _Exits = new Queue<Line>();
            _SocketSendable = socket_sendable;
            _SocketRecevieable = socket_recevieable;
            _Messages = new Queue<SocketMessage>();
        }

        public event Action<Line> JoinStreamEvent;
        public event Action<Line> LeftStreamEvent;        

        bool IUpdatable<Timestamp>.Update(Timestamp time)
        {            
            _HandleReceive();
            _HandleSend();
            _HandleTimeout(time);
            
            _HandleDisconnect();
            return true;
        }

        private void _HandleSend()
        {
            SocketMessage message = null;
            while ((message = _Messages.SafeDequeue()) != null)
            {
                if (message.IsError() == false)
                {
                    var line = _Query(message.RemoteEndPoint);
                    line.MessageSendResult(message);
                }
                else
                {
                    _HandleErrorDisconnect(message.RemoteEndPoint);
                }
            }
        }

        private void _HandleDisconnect()
        {
            if (_Exits.Count > 0)
            {
                var line = _Exits.Dequeue();
                
                _Remove(line);                
            }            
        }

        private void _HandleErrorDisconnect(EndPoint end_point)
        {
            
            Line line;
            if (_Lines.TryGetValue(end_point, out line))
            {
                _Remove(line);
            }
        }

        private void _SendOut(SocketMessage message)
        {
            _Messages.SafeEnqueue(message);

            
        }
        private void _HandleReceive()
        {
            var packages = _SocketRecevieable.Received();
            

            for (int i = 0; i < packages.Length; i++)
            {
                var package = packages[i];
                if (package.IsError() == false)
                {
                    var line = _Query(package.RemoteEndPoint);
                    line.Input(package);
                }
                else
                {
                    _HandleErrorDisconnect(package.RemoteEndPoint);                    
                }
                
            }
        }

        private void _JoinLine(Line line)
        {
            _Logger.Register(line);
            line.OutputEvent += _AddOutputPackageHandler;            
            JoinStreamEvent(line);
        }

        private void _HandleTimeout(Timestamp time)
        {
            var lines = _Lines.Values;

            var timeOutLines = new List<Line>();
            foreach (var line in lines)
            {
                if (line.Tick(time))
                {
                    timeOutLines.Add(line);
                }                
            }
            foreach (var timeOutLine in timeOutLines)
            {
                _Remove(timeOutLine);
            }


        }
        
        private void _Remove(Line time_out_line)
        {
            _LeftLine(time_out_line);
            _Lines.Remove(time_out_line.EndPoint);
        }

        private void _LeftLine(Line line)
        {
            _Logger.Unregister(line);
            line.OutputEvent -= _AddOutputPackageHandler;            
            LeftStreamEvent(line);
        }

        private void _AddOutputPackageHandler(SocketMessage message)
        {            
            _SocketSendable.Transport(message);
        }

        private Line _Query(EndPoint end_point)
        {
            Line line ;            
            if(_Lines.TryGetValue(end_point, out line) == false)
            {
                // new line
                line = new Line(end_point);                
                _Add(line);
            }
            return line;
        }

        private void _Add(Line line)
        {
            _JoinLine(line);
            _Lines.Add(line.EndPoint, line);
        }

        void IBootable.Launch()
        {
            _Logger.Start();


            _SocketSendable.DoneEvent += _SendOut;
        }

        

        void IBootable.Shutdown()
        {
            _SocketSendable.DoneEvent -= _SendOut;
            _Logger.End();
        }

        

        public void Create(EndPoint end_point)
        {
            if (_Lines.ContainsKey(end_point))
                throw new Exception("Already existing lines.");
            var line = new Line(end_point);
            
            _Add(line);            

            
        }

        public void Destroy(EndPoint end_point)
        {
            Line line;
            if(_Lines.TryGetValue(end_point, out line))
                _Exits.Enqueue(line);

        }
    }
}