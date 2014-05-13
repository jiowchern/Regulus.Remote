using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class Level : Regulus.Utility.IUpdatable
    {
        Queue<string> _MapNames;
        public delegate void OnDone();
        public event OnDone DoneEvent;

        //IMap Current { get; private set; }
        bool Utility.IUpdatable.Update()
        {
            throw new NotImplementedException();
        }

        void Framework.ILaunched.Launch()
        {
            /*if (_MapNames.Count > 0)
                _Change(_Pop(_MapNames.Dequeue()));
            else
                DoneEvent();*/
        }

        private IMap _Pop(string map_name)
        {
            return new Map(map_name, LocalTime.Instance);
        }

        

        void Framework.ILaunched.Shutdown()
        {
            throw new NotImplementedException();
        }
    }
}
