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
        Regulus.Utility.TimeCounter _IdleTime;
        bool _NeedRecover;
        public Scene(Team team, Zone zone, Remoting.Time time , bool need_recovery)
        {
            _Team = team;
            _Id = Guid.NewGuid();
            this._Zone = zone;
            this._Time = time;

            _Updater = new Utility.Updater();
            _NeedRecover = need_recovery;
        }
        
        bool Utility.IUpdatable.Update()
        {
            if (_NeedRecover)
            {
                if (_Team.MemberAmount() == 0)
                {
                    if (_IdleTime == null)
                    {
                        _IdleTime = new Utility.TimeCounter();
                    }
                    else if (_IdleTime.Second >= 30f)
                    {
                        return false;
                    }
                }
                else
                {
                    _IdleTime = null;
                }
            }
            
            
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
