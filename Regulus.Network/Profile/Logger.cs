using Regulus.Utility;
using System.Collections.Generic;
using System.Threading;
using QueueThreadHelper = Regulus.Extension.QueueThreadHelper;

namespace Regulus.Network.Profile
{
    public class Logger
    {
        private readonly int m_Sample;
        public static bool Enable = false;

        private class Command
        {
            public bool Add;
            public Line Line;
        }

        private readonly System.Collections.Generic.Queue<Command> m_Commands;


        private readonly List<Line> m_Lines;

        private volatile bool m_Enable;


        public Logger(int Sample)
        {
            m_Sample = Sample;

            m_Commands = new System.Collections.Generic.Queue<Command>();

            m_Lines = new List<Line>();
        }
        public void Register(Line Line)
        {
            Command command = new Command();
            command.Add = true;
            command.Line = Line;
            QueueThreadHelper.SafeEnqueue(m_Commands, command);
        }

        public void Unregister(Line Line)
        {
            Command command = new Command();
            command.Add = false;
            command.Line = Line;
            QueueThreadHelper.SafeEnqueue(m_Commands, command);
        }

        public void Start()
        {
            m_Enable = true;
            ThreadPool.QueueUserWorkItem(Collect);
        }

        private void Collect(object State)
        {



            while (m_Enable)
            {
                Command command = null;
                while ((command = QueueThreadHelper.SafeDequeue(m_Commands)) != null)
                    if (command.Add)
                        m_Lines.Add(command.Line);
                    else
                        m_Lines.Remove(command.Line);
                if (Enable)
                {
                    foreach (Line line in m_Lines)
                        WriteLog(line);
                }


                System.Threading.Thread.Sleep(m_Sample);
            }

        }

        private void WriteLog(Line Line)
        {

            string logstring = string.Format(
                "[RUDP] RemoteEndPoint:{0} SendBytes:{1} ReceiveBytes:{2} SRTT:{3} RTO:{4} SendPackages:{5} SendLost:{6} ReceivePackages:{7} ReceiveInvalidPackages:{8} LastRTT:{9} SendBlock:{10} LastRTO:{11} ReceiveBlock:{12} ReceiveNumber:{13} SendNumber:{14}",
                Line.EndPoint, Line.SendBytes, Line.ReceiveBytes, Line.Srtt, Line.Rto, Line.SendedPackages, Line.SendLostPackages, Line.ReceivePackages, Line.ReceiveInvalidPackages, Line.LastRtt, Line.SendBlock, Line.LastRto, Line.ReceiveBlock, Line.ReceiveNumber, Line.SendNumber);

            Log.Instance.WriteInfo(logstring);
        }


        public void End()
        {
            m_Enable = false;
        }
    }
}
