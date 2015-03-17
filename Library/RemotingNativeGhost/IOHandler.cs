using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Ghost.Native
{    

    class IOHandler : Regulus.Utility.Singleton<IOHandler>
    {
        
       
        int _ThreadCount;
        
        
        object _Sync;
        Regulus.Utility.Updater _Updater;
        public IOHandler()
        {
            _Sync = new object();
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
            lock (_Sync)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(_Handle));
                _ThreadCount++;
                System.Diagnostics.Debug.WriteLine("IOHandler Threads:" + _ThreadCount);
            }
            
        }

        private void _Handle(object obj)
        {
            Regulus.Utility.TimeCounter counter = new Utility.TimeCounter();
            do
            {
                
                _Updater.Update();                


                if(counter.Second > 1)
                {
                    System.Threading.Thread.Sleep(0);
                    counter.Reset();
                }
                

                
            } while (_Updater.Count > 0);            
            
            _Shutdown();            
        }

        private void _Shutdown()
        {
            lock (_Sync)
            {

                _ThreadCount--;
                System.Diagnostics.Debug.WriteLine("IOHandler Threads:" + _ThreadCount);
                
            }
        }
    }
}
