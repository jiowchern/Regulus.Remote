using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{
    public class PackageRecorder : Regulus.Utility.IUpdatable
    {
        public delegate void ChangeCallback();
        public event ChangeCallback ChangeEvent;
        public Int64 TotalBytes { get; private set; }


        public Int64 _SecondBytes;
        public Int64 SecondBytes { get; private set; }
        Regulus.Utility.TimeCounter _Counter;

        public PackageRecorder()
        {
            _Counter = new Utility.TimeCounter();
        }

        internal void Set(int size)
        {
            lock (_Counter)
            {
                TotalBytes += size;
                _SecondBytes += size;                
                ChangeEvent();
            }
            
        }

        bool Utility.IUpdatable.Update()
        {
            lock (_Counter)
            {
                if (_Counter.Second > 1)
                {
                    SecondBytes = _SecondBytes;
                    _SecondBytes = 0;
                    _Counter.Reset();
                }
            }
            
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            lock (_Counter)
                _Counter.Reset();
        }

        void Framework.ILaunched.Shutdown()
        {
            lock (_Counter)
                SecondBytes = 0;
        }
    }

    public class NetworkMonitor : Regulus.Utility.Singleton<NetworkMonitor>
    {
        public PackageRecorder Read { get;private set; }
        public PackageRecorder Write { get; private set; }

        
        volatile  bool _ThreadEnable = false;
        volatile bool _Reset = false;
        public NetworkMonitor()
        {
            Read = new PackageRecorder();
            Read.ChangeEvent += _ResetTime;
            Write = new PackageRecorder();
            Write.ChangeEvent += _ResetTime;
        }

        void _ResetTime()
        {
            if(_ThreadEnable == false)
            {
                _ThreadEnable = true;
                System.Threading.ThreadPool.QueueUserWorkItem(_Update);
                
            }

            _Reset = true;
        }

        private void _Update(object state)
        {
            var updater = new Regulus.Utility.CenterOfUpdateable();
            updater.Add(Read);
            updater.Add(Write);

            
            Regulus.Utility.TimeCounter counter = new Utility.TimeCounter();
            do
            {

                updater.Working();
                
                    
                if (_Reset)
                {
                    counter.Reset();
                    _Reset = false;
                }

                System.Threading.Thread.Sleep(1000);
            }
            while (counter.Second <= 30);
            updater.Shutdown();
            _ThreadEnable = false;

            
        }
    }
}

