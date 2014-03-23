using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    class BotSelectActorStage : Regulus.Game.IStage
    {
        private Regulus.Project.SamebestKeys.IUser _User;
        private string account;
        public event Action<string> ResultEvent;
        public BotSelectActorStage(Regulus.Project.SamebestKeys.IUser _User, string account)
        {
            // TODO: Complete member initialization
            this._User = _User;
            this.account = account;
        }
        void Regulus.Game.IStage.Enter()
        {
            _User.ParkingProvider.Supply += ParkingProvider_Supply;
        }

        void ParkingProvider_Supply(Regulus.Project.SamebestKeys.IParking obj)
        {
            obj.Select(account + "-1").OnValue += ResultEvent;
        }

        void Regulus.Game.IStage.Leave()
        {
            _User.ParkingProvider.Supply -= ParkingProvider_Supply;
        }

        void Regulus.Game.IStage.Update()
        {
            
        }
    }
}
