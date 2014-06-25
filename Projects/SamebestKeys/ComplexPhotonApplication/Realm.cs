using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys.Dungeons
{
    interface IScene
    {
        event Action ShutdownEvent;
        Guid Id { get; }
        bool Join(Member player);
        void Exit(Member player);
    }

    class Scene : Regulus.Utility.IUpdatable , IScene
    {
        Guid _Id;
        private Remoting.Time _Time;
        private Zone _Zone;
        Team _Team;
        Regulus.Utility.Updater _Updater;
        
        public Scene(Team team, Zone zone, Remoting.Time time )
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

        event Action _ShutdownEvent;
        
        event Action IScene.ShutdownEvent
        {
            add { _ShutdownEvent += value;  }
            remove { _ShutdownEvent -= value; }
        }

        Guid IScene.Id
        {
            get { return _Id; }
        }

        bool IScene.Join(Member player)
        {
            return _Team.Join(player);            
        }

        void IScene.Exit(Member player)
        {
            _Team.Left(player);            
        }
    }
    
    
}
