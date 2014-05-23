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
        Remoting.Value<bool> Join(Player player);
        void Exit(Player player);
    }

    partial class Realm : Regulus.Utility.IUpdatable , IRealm
    {
        Guid _Id;
        private Remoting.Time _Time;
        private Zone _Zone;
        Team _Team;
        Regulus.Utility.Updater _Updater;
        
        Realm(Team team, Zone zone, Remoting.Time time )
        {
            _Team = team;
            _Id = Guid.NewGuid();
            this._Zone = zone;
            this._Time = time;

            _Updater = new Utility.Updater();
        }
        
        bool Utility.IUpdatable.Update()
        {
            _Updater.Update();
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            _Updater.Add(_Zone);
            _Updater.Add(_Team);
        }
        void Framework.ILaunched.Shutdown()
        {
            _Updater.Shutdown();
        }

        event Action IRealm.ShutdownEvent
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        Guid IRealm.Id
        {
            get { return _Id; }
        }

        Remoting.Value<bool> IRealm.Join(Player player)
        {
            return _Team.Join(player);            
        }

        void IRealm.Exit(Player player)
        {
            _Team.Left(player);
        }
    }
    
    
}
