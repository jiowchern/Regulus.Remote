using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{

    interface ILevel
    {
        event Action ShutdownEvent;
        Guid Id { get; }

        Remoting.Value<IMap> QueryCurrent();
    }

    class Level : Regulus.Utility.IUpdatable , ILevel
    {
        Queue<string> _MapNames;
        public delegate void OnDone();
        public event OnDone DoneEvent;

        private Remoting.Time _Time;

        public Level(Remoting.Time time)
        {            
            this._Time = time;
        }
        bool Utility.IUpdatable.Update()
        {
            throw new NotImplementedException();
        }

        void Framework.ILaunched.Launch()
        {
            if (_MapNames.Count > 0)
                _Change(_Create(_MapNames.Dequeue()));
            else
                DoneEvent();
        }

        private void _Change(IMap map)
        {
            throw new NotImplementedException();
        }

        private IMap _Create(string map_name)
        {
            return new Map(map_name, LocalTime.Instance);
        }

        

        void Framework.ILaunched.Shutdown()
        {
            throw new NotImplementedException();
        }

        event Action ILevel.ShutdownEvent
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        internal void Build(Data.Level level)
        {
            throw new NotImplementedException();
        }


        Guid ILevel.Id
        {
            get { throw new NotImplementedException(); }
        }


        Remoting.Value<IMap> ILevel.QueryCurrent()
        {
            throw new NotImplementedException();
        }
    }
}
