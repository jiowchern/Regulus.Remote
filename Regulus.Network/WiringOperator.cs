using Regulus.Network.Package;
using Regulus.Network.Profile;
using Regulus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Regulus.Network
{
    public class WiringOperator : IUpdatable<Timestamp>
    {
        private readonly ISocketSendable m_SocketSendable;
        private readonly ISocketRecevieable m_SocketRecevieable;
        private readonly bool _Listener;


        private readonly Dictionary<EndPoint, Line> m_Lines;
        private readonly System.Collections.Generic.Queue<Line> m_Exits;

        private readonly Logger m_Logger;
        public WiringOperator(ISocketSendable SocketSendable, ISocketRecevieable SocketRecevieable, bool listener)
        {
            m_Logger = new Logger(100);
            m_Lines = new Dictionary<EndPoint, Line>();
            m_Exits = new System.Collections.Generic.Queue<Line>();
            m_SocketSendable = SocketSendable;
            m_SocketRecevieable = SocketRecevieable;
            _Listener = listener;




        }

        public event Action<Line> JoinStreamEvent;
        public event Action<Line> LeftStreamEvent;

        bool IUpdatable<Timestamp>.Update(Timestamp Time)
        {
            HandleReceive();

            HandleTimeout(Time);

            HandleDisconnect();
            return true;
        }



        private void HandleDisconnect()
        {
            if (m_Exits.Count > 0)
            {
                Line line = m_Exits.Dequeue();

                Remove(line);
            }
        }

        private void HandleErrorDisconnect(EndPoint EndPoint)
        {

            Line line;
            if (m_Lines.TryGetValue(EndPoint, out line))
                Remove(line);
        }

        private void SendOut(SocketMessage Message)
        {
            m_SocketSendable.Transport(Message);



        }
        private void HandleReceive()
        {
            SocketMessage[] packages = m_SocketRecevieable.Received();


            for (int i = 0; i < packages.Length; i++)
            {
                SocketMessage package = packages[i];
                if (package.IsError() == false)
                {
                    Line line = Find(package.RemoteEndPoint);
                    if (line != null)
                        line.Input(package);
                }
                else
                {
                    HandleErrorDisconnect(package.RemoteEndPoint);
                }

            }
        }

        private void JoinLine(Line Line)
        {
            m_Logger.Register(Line);
            Line.OutputEvent += SendOut;
            JoinStreamEvent(Line);
        }

        private void HandleTimeout(Timestamp Time)
        {
            Line[] lines = m_Lines.Values.ToArray();

            List<Line> timeOutLines = new List<Line>();
            foreach (Line line in lines)
                if (line.Tick(Time))
                    timeOutLines.Add(line);
            foreach (Line timeOutLine in timeOutLines)
                Remove(timeOutLine);
        }

        private void Remove(Line TimeOutLine)
        {
            LeftLine(TimeOutLine);
            m_Lines.Remove(TimeOutLine.EndPoint);
        }

        private void LeftLine(Line Line)
        {
            m_Logger.Unregister(Line);
            Line.OutputEvent -= SendOut;
            LeftStreamEvent(Line);
        }



        private Line Find(EndPoint EndPoint)
        {
            Line line;
            if (m_Lines.TryGetValue(EndPoint, out line) == false && _Listener)
            {
                // new line
                line = new Line(EndPoint);
                Add(line);
            }
            return line;
        }

        private void Add(Line Line)
        {
            JoinLine(Line);
            m_Lines.Add(Line.EndPoint, Line);
        }

        void IBootable.Launch()
        {
            m_Logger.Start();



        }



        void IBootable.Shutdown()
        {

            m_Logger.End();
        }



        public void Create(EndPoint EndPoint)
        {
            if (m_Lines.ContainsKey(EndPoint))
                throw new Exception("Already existing lines.");
            Line line = new Line(EndPoint);

            Add(line);


        }

        public void Destroy(EndPoint EndPoint)
        {
            Line line;
            if (m_Lines.TryGetValue(EndPoint, out line))
                m_Exits.Enqueue(line);

        }
    }
}