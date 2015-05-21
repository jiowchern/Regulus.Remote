using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Ghost.Native
{    

    class IOHandler : Regulus.Utility.Singleton<IOHandler>
    {
        Regulus.Utility.PowerRegulator _PowerRegulator;


        Regulus.Utility.FPSCounter _Fps;

        public int Fps { get { return _PowerRegulator.FPS; } }
        public float Power { get { return _PowerRegulator.Power; } }
        volatile bool _ThreadEnable;
        
        Regulus.Utility.CenterOfUpdateable _Updater;
        public IOHandler()
        {            
            _PowerRegulator = new Utility.PowerRegulator(30);
            _Updater = new Utility.CenterOfUpdateable();
            
        }
        public void Stop(Regulus.Utility.IUpdatable updater)
        {
            _Updater.Remove(updater);
        
        }
        public void Start(Regulus.Utility.IUpdatable updater)
        {
            
            _Updater.Add(updater);
            _Launch();
         
        }

        private void _Launch()
        {            
            if (_ThreadEnable == false)
            {
                _ThreadEnable = true;
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(_Handle));
            }
        }

        private void _Handle(object obj)
        {
            Regulus.Utility.SpinWait sw = new Regulus.Utility.SpinWait();
            long response = 0;
            do
            {
                var current = Agent.ResponseQueueCount + Agent.RequestQueueCount;
                _PowerRegulator.Operate(current);                

                response = current;
                _Updater.Working();
                
            } while (_Updater.Count > 0);            
            
            _Shutdown();            
        }

        private void _Shutdown()
        {
            _ThreadEnable = false;
        }
    }
}
