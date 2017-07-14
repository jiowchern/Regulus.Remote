using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Regulus.Framework;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    public class WiringOperator : IUpdatable<Timestamp>
    {
        private readonly ISendable _Sendable;
        private readonly IRecevieable _Recevieable;

        private readonly Dictionary<EndPoint , Line> _Lines;
        private readonly Queue<Line> _Exits;

        public WiringOperator(ISendable sendable , IRecevieable recevieable)
        {
            _Lines = new Dictionary<EndPoint, Line>();
            _Exits = new Queue<Line>();
            _Sendable = sendable;
            _Recevieable = recevieable;
        }

        public event Action<ILine> JoinStreamEvent;
        public event Action<ILine> LeftStreamEvent;        

        bool IUpdatable<Timestamp>.Update(Timestamp time)
        {            
            _HandleReceive();            
            _HandleTimeout(time);
            _HandleErrorDisconnect();
            _HandleDisconnect();
            return true;
        }

        private void _HandleDisconnect()
        {
            if (_Exits.Count > 0)
            {
                var line = _Exits.Dequeue();
                _Remove(line);                
            }            
        }

        private void _HandleErrorDisconnect()
        {
            var endPoints = _Recevieable.ErrorPoints();


            for (int i = 0; i < endPoints.Length; i++)
            {
                var endPoint = endPoints[i];
                Line line;
                if (_Lines.TryGetValue(endPoint, out line))
                {
                    _Remove(line);
                }
            }            
        }


        private void _HandleReceive()
        {
            var packages = _Recevieable.Received();
            

            for (int i = 0; i < packages.Length; i++)
            {
                var package = packages[i];
                var pkg = new SegmentPackage(package.Segment);
                var line = _Query(package.EndPoint);                
                line.Input(pkg);
            }
        }

        private void _JoinLine(Line line)
        {
            line.OutputEvent += _AddOutputPackageHandler;            
            JoinStreamEvent(line);
        }

        private void _HandleTimeout(Timestamp time)
        {
            var lines = _Lines.Values;

            var timeOutLines = new List<Line>();
            foreach (var line in lines)
            {
                if (line.IsTimeout(time))
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
            line.OutputEvent -= _AddOutputPackageHandler;            
            LeftStreamEvent(line);
        }

        private void _AddOutputPackageHandler(EndPoint arg1, byte[] arg2)
        {
            var package = new SocketPackage();
            package.EndPoint = arg1;
            package.Segment = arg2;
            _Sendable.Transport(package);
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
            
        }

        

        void IBootable.Shutdown()
        {
            
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