using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Net;
using Regulus.Framework;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    public class EmptyArray<T>
    {
        private readonly static T[] _Empty = new T[0];
        public static implicit operator T[] (EmptyArray<T> ea)
        {
            return _Empty;
        }
    }
    public class StreamProvider : IUpdatable<Timestamp>
    {
        private readonly ISendable _Sendable;
        private readonly IRecevieable _Recevieable;

        private readonly List<SocketPackage> _ReceivePackages;
        

        private readonly Dictionary<EndPoint , Line> _Lines;

        private readonly SocketPackage[] _Empty;

        public StreamProvider(ISendable sendable , IRecevieable recevieable)
        {
            _Lines = new Dictionary<EndPoint, Line>();
            _Empty = new EmptyArray<SocketPackage>();
            _ReceivePackages = new List<SocketPackage>();
            
            _Sendable = sendable;
            _Recevieable = recevieable;
        }

        public event Action<IStream> JoinStreamEvent;
        public event Action<IStream> LeftStreamEvent;        

        bool IUpdatable<Timestamp>.Update(Timestamp time)
        {
            _HandleReceive();            
            _HandleTimeout(time);
            return true;
        }

        

        private void _HandleReceive()
        {
            var packages = _Empty;
            lock (_ReceivePackages)
            {
                packages = _ReceivePackages.ToArray();
                _ReceivePackages.Clear();
            }

            for (int i = 0; i < packages.Length; i++)
            {
                var package = packages[i];
                var line = _Find(package.EndPoint);
                if (line == null)
                {
                    // new line
                    line = new Line(package.EndPoint);
                    _JoinLine(line);
                    _Lines.Add(line.EndPoint, line);
                }

                line.Input(package.Buffer);
            }
        }

        private void _JoinLine(Line line)
        {
            line.OutputEvent += AddOutputPackageHandler;            
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
                _LeftLine(timeOutLine);
                _Lines.Remove(timeOutLine.EndPoint);
            }


        }

        private void _LeftLine(Line line)
        {
            line.OutputEvent -= AddOutputPackageHandler;            
            LeftStreamEvent(line);
        }

        private void AddOutputPackageHandler(EndPoint arg1, byte[] arg2)
        {
            var package = new SocketPackage();
            package.EndPoint = arg1;
            package.Buffer = arg2;
            _Sendable.Transport(package);
        }

        private Line _Find(EndPoint end_point)
        {
            Line line ;            
            _Lines.TryGetValue(end_point, out line);
            return line;
        }

        void IBootable.Launch()
        {
            _Recevieable.ReceivedEvent += _ReceiveHandler;
        }

        private void _ReceiveHandler(SocketPackage obj)
        {
            lock (_ReceivePackages)
            {
                _ReceivePackages.Add(obj);
            }            
        }

        void IBootable.Shutdown()
        {
            _Recevieable.ReceivedEvent -= _ReceiveHandler;
        }
    }
}