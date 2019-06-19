using System;
using System.Collections.Generic;


using Regulus.Utility;

namespace Regulus.Remote.Soul
{
    internal class ThreadCoreHandler
    {
        private readonly Queue<ISoulBinder> _Binders;

        private readonly ICore _Core;

        private readonly IProtocol _Protocol;

        private readonly ICommand _Command;

        private readonly Updater _RequesterHandlers;

        private readonly PowerRegulator _Spin;

        private readonly AutoPowerRegulator _AutoPowerRegulator;

        private volatile bool _Run;

        public event Action ShutdownEvent;

        public int FPS
        {
            get { return _Spin.FPS; }
        }

        public float Power
        {
            get { return _Spin.Power; }
        }

        public ThreadCoreHandler(ICore core , IProtocol protocol , ICommand command)
        {
            if(core == null)
            {
                throw new ArgumentNullException(nameof(core));
            }

            _Core = core;
            _Protocol = protocol;
            _Command = command;

            _RequesterHandlers = new Updater();
            _Spin = new PowerRegulator();
            _AutoPowerRegulator = new AutoPowerRegulator(_Spin);
            _Binders = new Queue<ISoulBinder>();
        }

        public void DoWork(object obj)
        {
            Singleton<Log>.Instance.WriteInfo("server core launch");
            _Run = true;
            _Core.Launch(_Protocol , _Command);

            while(_Run)
            {
                if(_Binders.Count > 0)
                {
                    lock(_Binders)
                    {
                        while(_Binders.Count > 0)
                        {
                            var provider = _Binders.Dequeue();
                            _Core.AssignBinder(provider);
                        }
                    }
                }

                
                _RequesterHandlers.Working();
                _AutoPowerRegulator.Operate();
            }

            _Core.Shutdown();
            ShutdownEvent();
            Singleton<Log>.Instance.WriteInfo("server core shutdown");
        }

        public void Stop()
        {
            _Run = false;
        }

        internal void Push(ISoulBinder soulBinder, IUpdatable handler)
        {
            _RequesterHandlers.Add(handler);

            lock(_Binders)
            {
                _Binders.Enqueue(soulBinder);
            }
        }
    }
}