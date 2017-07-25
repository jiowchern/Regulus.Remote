using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Regulus.Extension;
using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Network.Profile
{
    class Logger
    {
        private readonly float _Sample;

        class Command
        {
            public bool Add;
            public Line Line;
        }

        private readonly Queue<Command> _Commands;
        

        private readonly List<Line> _Lines;

        private volatile bool _Enable;

        
        public Logger(float sample)
        {
            _Sample = sample;

            _Commands = new Queue<Command>();

            _Lines = new List<Line>();
        }
        public void Register(Line line)
        {
            var command = new Command();
            command.Add = true;
            command.Line = line;
            _Commands.SafeEnqueue(command);
        }

        public void Unregister(Line line)
        {
            var command = new Command();
            command.Add = false;
            command.Line = line;
            _Commands.SafeEnqueue(command);
        }

        public void Start()
        {
            _Enable = true;
            ThreadPool.QueueUserWorkItem(_Collect);
        }

        private void _Collect(object state)
        {
            Regulus.Utility.TimeCounter timeCounter = new TimeCounter();
            Regulus.Utility.AutoPowerRegulator regulator = new AutoPowerRegulator(new PowerRegulator());

            while (_Enable)
            {
                Command command = null;
                while ((command = _Commands.SafeDequeue()) != null)
                {
                    if (command.Add)
                    {
                        _Lines.Add(command.Line);
                    }
                    else
                        _Lines.Remove(command.Line);
                }
                if (timeCounter.Second > _Sample)
                {
                    
                    foreach (var line in _Lines)
                    {
                        _WriteLog(line);
                    }
                    timeCounter.Reset();
                }
                regulator.Operate();
            }
            
        }

        private void _WriteLog(Line line)
        {

            var logstring = string.Format(
                "[RUDP] EndPoint:{0} SendBytes:{1} ReceiveBytes:{2} SRTT:{3} RTO:{4} SendPackages:{5} SendLost:{6} ReceivePackages:{7} ReceiveInvalidPackages:{8} LastRTT:{9}",
                line.EndPoint , line.SendBytes , line.ReceiveBytes , line.SRTT , line.RTO , line.SendedPackages , line.SendLostPackages , line.ReceivePackages , line.ReceiveInvalidPackages , line.LastRTT);

            Regulus.Utility.Log.Instance.WriteInfo(logstring);
        }


        public void End()
        {
            _Enable = false;
        }
    }
}
