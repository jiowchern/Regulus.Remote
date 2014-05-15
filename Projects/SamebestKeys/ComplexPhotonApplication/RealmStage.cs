using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    partial class RealmStage : Regulus.Game.IStage
    {
        private IRealm _Realm;
        Regulus.Game.StageMachine _Machine;
        Player[] _Players;
        public RealmStage(IRealm realm ,Regulus.Project.SamebestKeys.Serializable.DBEntityInfomation[] actors )
        {            
            this._Realm = realm;
            _Machine = new Game.StageMachine();

            _Players = (from actor in actors select new Player(actor)).ToArray();
        }

        void Game.IStage.Enter()
        {
            
        }

        void Game.IStage.Leave()
        {
            
        }

        void Game.IStage.Update()
        {
            _Machine.Update();
        }
    }


    partial class RealmStage 
    {
        class WaitStartStage : Regulus.Game.IStage
        {
            private IRealm _Realm;
            Player[] _Players;
            void Game.IStage.Enter()
            {
                var result = _Realm.Join(_Players);
               
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
    
}
