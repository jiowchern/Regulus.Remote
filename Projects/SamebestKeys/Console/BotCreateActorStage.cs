using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    class BotCreateActorStage : Regulus.Utility.IStage
    {
        private Regulus.Project.SamebestKeys.IUser _User;
        public event Action<bool> ResultEvent;
        string _Account;
        public BotCreateActorStage(Regulus.Project.SamebestKeys.IUser _User, string account)
        {
            _Account = account;
            this._User = _User;
        }
        void Regulus.Utility.IStage.Enter()
        {
            _User.ParkingProvider.Supply += ParkingProvider_Supply;
        }

        void ParkingProvider_Supply(Regulus.Project.SamebestKeys.IParking obj)
        {            
            obj.CreateActor(new Regulus.Project.SamebestKeys.Serializable.EntityLookInfomation() { Name = _Account + "-1" } , Regulus.Project.SamebestKeys.Serializable.EntityPropertyInfomation.IDENTITY.GUEST).OnValue += ResultEvent;
        }

        void Regulus.Utility.IStage.Leave()
        {
            _User.ParkingProvider.Supply -= ParkingProvider_Supply;            
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }
    }
}
