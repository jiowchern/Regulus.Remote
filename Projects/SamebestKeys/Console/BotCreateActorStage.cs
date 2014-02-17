using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    class BotCreateActorStage : Regulus.Game.IStage
    {
        private Regulus.Project.SamebestKeys.IUser _User;
        public event Action<bool> ResultEvent;
        string _Account;
        public BotCreateActorStage(Regulus.Project.SamebestKeys.IUser _User, string account)
        {
            _Account = account;
            this._User = _User;
        }
        void Regulus.Game.IStage.Enter()
        {
            _User.ParkingProvider.Supply += ParkingProvider_Supply;
        }

        void ParkingProvider_Supply(Regulus.Project.SamebestKeys.IParking obj)
        {            
            obj.CreateActor(new Regulus.Project.SamebestKeys.Serializable.EntityLookInfomation() { Name = _Account + "-1" }).OnValue += ResultEvent;
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
