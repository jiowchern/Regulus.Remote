using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Ghost.Native
{    

    class IOHandler : Regulus.Utility.Singleton<IOHandler>
    {

        System.Threading.Thread _Thread;
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
                if (_Thread == null)
                {
                    _Thread = new System.Threading.Thread(_Handle);
                    _Thread.IsBackground = true;

                    _Thread.Start();
                    _ThreadCount++;
                    System.Diagnostics.Debug.WriteLine("IOHandler Threads:" + _ThreadCount);
                }
            }
            
        }

        private void _Handle(object obj)
        {
            
            do
            {
                _Updater.Update();
                System.Threading.Thread.Sleep(0);
            } while (_Updater.Count > 0);            
            
            _Shutdown();            
        }

        private void _Shutdown()
        {
            lock (_Sync)
            {

                if (_Thread != null)
                {
                    var thread = _Thread;
                    _Thread = null;
                    _ThreadCount--;
                    System.Diagnostics.Debug.WriteLine("IOHandler Threads:" + _ThreadCount);
                    //thread.Abort();
                    //thread.Join();
                }
                
            }
        }
    }
}
