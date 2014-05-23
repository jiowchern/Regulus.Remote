using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    partial class RealmStage : Regulus.Game.IStage
    {
        private IRealm _Realm;        
        Player[] _Players;
        public RealmStage(IRealm realm ,Regulus.Project.SamebestKeys.Serializable.DBEntityInfomation[] actors )
        {            
            this._Realm = realm;
            
            _Players = (from actor in actors select new Player(actor)).ToArray();
        }

        void Game.IStage.Enter()
        {
            _Realm.Join(_Players[0]);
        }

        void Game.IStage.Leave()
        {
            _Realm.Exit(_Players[0]);
        }

        void Game.IStage.Update()
        {            
        }
    }

    partial class Realm
    {
        class PlayingStage : Regulus.Game.IStage
        {
            private Zone _Zone;
            
            public delegate void OnDone();
            public event OnDone DoneEvent;

            Regulus.Utility.Updater _ZoneUpdater;
            public PlayingStage(Zone zone)
            {                
                this._Zone = zone;                
                _ZoneUpdater = new Utility.Updater();
            }
            void Game.IStage.Enter()
            {
                _ZoneUpdater.Add(_Zone);
            }

            void Game.IStage.Leave()
            {
                _ZoneUpdater.Shutdown();
            }

            void Game.IStage.Update()
            {
                _ZoneUpdater.Update();
            }
        }
    }

    partial class Realm
    {
        class ReadyStage : Regulus.Game.IStage
        {
            public delegate void OnDone();
            public event OnDone DoneEvent;
            private JoinCondition _JoinCondition;
            

            public ReadyStage()
            {                
                
            }

            public ReadyStage(JoinCondition join_condition)
            {
                // TODO: Complete member initialization
                this._JoinCondition = join_condition;
            }

            void Game.IStage.Enter()
            {
                
            }

            void Game.IStage.Leave()
            {
                
            }

            void Game.IStage.Update()
            {
                if (_JoinCondition.LastCheck() == false)
                {
                    DoneEvent();
                }
            }
        }
    }
   
    
}
