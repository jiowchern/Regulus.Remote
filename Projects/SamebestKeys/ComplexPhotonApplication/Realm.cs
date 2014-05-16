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
        
        private Remoting.Time _Time;
        Data.Realm _Realm;
        
        event Action _ShutdownEvent;

        Regulus.Game.StageMachine _Machine;

        public Realm(Remoting.Time time)
        {
            _Machine = new Game.StageMachine();                   
            _Time = time;
            _Id = new Guid();
        }

        public Realm(Remoting.Time time, Data.Realm realm) : this(time)
        {
            _Realm = realm;            
        }
        bool Utility.IUpdatable.Update()
        {
            _Machine.Update();
            
            return true;
        }

        void Framework.ILaunched.Launch()
        {            
            _Build(_Realm);
            
        }

        private Map _Create(string map_name)
        {
            return new Map(map_name, _Time);
        }

        void Framework.ILaunched.Shutdown()
        {
            _Machine.Termination();
            
        }
        
        event Action IRealm.ShutdownEvent
        {
            add { _ShutdownEvent += value; }
            remove { _ShutdownEvent -= value; }
        }

        void _Build(Data.Realm realm)
        {            
            _Build(_Realm.Stages);
        }

        private void _Build(Data.Stage[] stages)
        {
            //Map[] maps = new Map[];
            foreach (var stage in stages)
            {
                var map = _Create(stage.MapName);                
            }
        }

        Guid IRealm.Id
        {
            get { return _Id; }
        }

        Remoting.Value<bool> IRealm.Join(Player[] players)
        {            
            /*if (_Controllers.Count < _Realm.NumberForPlayer)
            {
                _Controllers.Add(new Controller(players));                
                return true;
            }*/
            return false;
        }
    }
    partial class Realm
    {
        class ReadyStage : Regulus.Game.IStage
        {

            void Game.IStage.Enter()
            {
                throw new NotImplementedException();
            }

            void Game.IStage.Leave()
            {
                throw new NotImplementedException();
            }

            void Game.IStage.Update()
            {
                throw new NotImplementedException();
            }
        }
    }
    partial class Realm
    {
        struct Controller
        {
            private Player[] _Players;

            public Controller(Player[] players)
            {                
                this._Players = players;
            } 
            
        }
    }
}
