using System;
using System.Collections.Generic;


using Regulus.Utility;

namespace Regulus.Remoting.Soul.Native
{
    internal class ThreadCoreHandler
    {
        private readonly Queue<ISoulBinder> _Binders;

        private readonly ICore _Core;

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

        public ThreadCoreHandler(ICore core)
        {
            if(core == null)
            {
                throw new ArgumentNullException();
            }

            _Core = core;

            _RequesterHandlers = new Updater();
            _Spin = new PowerRegulator();
            _AutoPowerRegulator = new AutoPowerRegulator(_Spin);
            _Binders = new Queue<ISoulBinder>();
        }

        public void DoWork(object obj)
        {
            Singleton<Log>.Instance.WriteInfo("server core launch");
            _Run = true;
            _Core.Launch();

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

                _Run = _Core.Update();
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