using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{

    interface IRealm
    {
        event Action ShutdownEvent;
        Guid Id { get; }
        Remoting.Value<bool> Join(Player[] players);
    }

    partial class Realm : Regulus.Utility.IUpdatable , IRealm
    {
        Guid _Id;
        List<Record> _Records;        
        Regulus.Utility.Updater _Updater;
        private Remoting.Time _Time;
        Data.Realm _Realm;
        List<Controller> _Controllers;
        public Realm(Remoting.Time time)
        {
            _Controllers = new List<Controller>();
            _Updater = new Utility.Updater();
            _Records = new List<Record>();
            _Time = time;
            _Id = new Guid();
        }
        bool Utility.IUpdatable.Update()
        {
            _Updater.Update();
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            
        }
        


        private Map _Create(string map_name)
        {
            return new Map(map_name, LocalTime.Instance);
        }

        void Framework.ILaunched.Shutdown()
        {
            _Updater.Shutdown();
        }

        event Action _ShutdownEvent;
        event Action IRealm.ShutdownEvent
        {
            add { _ShutdownEvent += value; }
            remove { _ShutdownEvent -= value; }
        }

        internal void Build(Data.Realm realm)
        {
            _Realm = realm;
        }

        Guid IRealm.Id
        {
            get { return _Id; }
        }



        Remoting.Value<bool> IRealm.Join(Player[] players)
        {
            return false;
            /*if (_Controllers.Count < _Realm.NumberForPlayer)
            { 

            }*/
        }
    }

    partial class Realm
    {
        struct Controller
        { 

        }
    }
}
