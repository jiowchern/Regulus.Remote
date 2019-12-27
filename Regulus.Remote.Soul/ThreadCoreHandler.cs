using System;
using System.Collections.Generic;


using Regulus.Utility;

namespace Regulus.Remote.Soul
{
    internal class ThreadCoreHandler
    {
        private readonly System.Collections.Concurrent.ConcurrentQueue<IBinder> _Binders;

        private readonly IEntry _Core;

        private readonly Updater _RequesterHandlers;

        private readonly PowerRegulator _Spin;

        private readonly AutoPowerRegulator _AutoPowerRegulator;

        private volatile bool _Run;
        private readonly System.Threading.Tasks.Task _Task;

        public event Action ShutdownEvent;

        public int FPS
        {
            get { return _Spin.FPS; }
        }

        public float Power
        {
            get { return _Spin.Power; }
        }

        public ThreadCoreHandler(IEntry core)
        {
            if(core == null)
            {
                throw new ArgumentNullException(nameof(core));
            }

            _Core = core;            

            _RequesterHandlers = new Updater();
            _Spin = new PowerRegulator();
            _AutoPowerRegulator = new AutoPowerRegulator(_Spin);
            
            _Binders = new System.Collections.Concurrent.ConcurrentQueue<IBinder>();

            _Task = new System.Threading.Tasks.Task(this.DoWork);
        }

        public void DoWork()
        {
            Singleton<Log>.Instance.WriteInfo("server core launch");
            _Run = true;
            _Core.Launch();

            while(_Run)
            {

                IBinder binder;
                if (_Binders.TryDequeue(out binder))
                {
                    _Core.AssignBinder(binder);
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
            _Task.Wait();
        }

        internal void Push(IBinder soulBinder, IUpdatable handler)
        {
            _RequesterHandlers.Add(handler);

            _Binders.Enqueue(soulBinder);
        }

        internal void Start()
        {
            _Task.Start();
        }
    }
}