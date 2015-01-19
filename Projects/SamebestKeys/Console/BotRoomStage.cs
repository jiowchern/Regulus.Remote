using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    class BotRoomStage : Regulus.Utility.IStage
    {
        private Regulus.Project.SamebestKeys.IUser _User;
        public event Action DoneEvent;

        public BotRoomStage(Regulus.Project.SamebestKeys.IUser user)
        {            
            _User = user;
        }

        void Regulus.Utility.IStage.Enter()
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

        void Regulus.Utility.IStage.Leave()
        {
            _User.RealmJumperProvider.Supply -= RealmJumperProvider_Supply;
            _User.TraversableProvider.Supply -= TraversableProvider_Supply;
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }
    }
}
