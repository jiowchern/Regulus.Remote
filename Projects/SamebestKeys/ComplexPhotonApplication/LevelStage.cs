using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class LevelStage : Regulus.Game.IStage<User>, ILevelSelector
    {
        private Serializable.AccountInfomation _Account;

        public delegate void OnDone(string level);
        public event OnDone DoneEvent;

        public delegate void OnBack();
        public event OnBack BackEvent;
        public LevelStage(Serializable.AccountInfomation account)
        {
            _Account = account;
        }

        Game.StageLock Game.IStage<User>.Enter(User user)
        {
            user.Provider.Bind<ILevelSelector>(this);
            return null;
        }

        void Game.IStage<User>.Leave(User user)
        {
            user.Provider.Unbind<ILevelSelector>(this);   
        }

        void Game.IStage<User>.Update(User user)
        {
            
        }

        Remoting.Value<string[]> ILevelSelector.QueryLevels()
        {
            return _Account.LevelRecords;
        }

        Remoting.Value<bool> ILevelSelector.Select(string id)
        {            
            var result = (from level in _Account.LevelRecords where level == id select true).SingleOrDefault();
            if (result)
            {
                DoneEvent(id);
            }
            return result;
        }


        void ILevelSelector.Back()
        {
            BackEvent();
        }
    }
}
