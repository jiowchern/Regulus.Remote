using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Ghost.Native
{    

    class IOHandler : Regulus.Utility.Singleton<IOHandler>
    {
        
       
        volatile bool _ThreadEnable;
        
        Regulus.Utility.Updater _Updater;
        public IOHandler()
        {
            
            _Updater = new Utility.Updater();
            
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
            
            do
            {                
                _Updater.Update();
                System.Threading.Thread.SpinWait(1);
            } while (_Updater.Count > 0);            
            
            _Shutdown();            
        }

        private void _Shutdown()
        {
            _ThreadEnable = false;
        }
    }
}
