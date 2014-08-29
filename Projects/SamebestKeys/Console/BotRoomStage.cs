using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    class BotRoomStage : Regulus.Game.IStage
    {
        private Regulus.Project.SamebestKeys.IUser _User;
        public event Action DoneEvent;

        public BotRoomStage(Regulus.Project.SamebestKeys.IUser user)
        {            
            _User = user;
        }

        void Regulus.Game.IStage.Enter()
        {
            _User.RealmJumperProvider.Supply += RealmJumperProvider_Supply;
            _User.TraversableProvider.Supply += TraversableProvider_Supply;
        }

        void TraversableProvider_Supply(Regulus.Project.SamebestKeys.ITraversable obj)
        {            
            obj.Ready();
        }

        void RealmJumperProvider_Supply(Regulus.Project.SamebestKeys.IRealmJumper obj)
        {
            obj.Jump("SC_1A");
            DoneEvent();
        }

        void Regulus.Game.IStage.Leave()
        {
            _User.RealmJumperProvider.Supply -= RealmJumperProvider_Supply;
            _User.TraversableProvider.Supply -= TraversableProvider_Supply;
        }

        void Regulus.Game.IStage.Update()
        {
            
        }
    }
}
