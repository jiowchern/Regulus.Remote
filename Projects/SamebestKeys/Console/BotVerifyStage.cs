using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    class BotVerifyStage : Regulus.Utility.IStage
    {
        private Regulus.Project.SamebestKeys.IUser _User;
        private string account;
        public event Action<Regulus.Project.SamebestKeys.LoginResult> ResultEvent;
        public BotVerifyStage(Regulus.Project.SamebestKeys.IUser _User, string account)
        {
            // TODO: Complete member initialization
            this._User = _User;
            this.account = account;
        }
        void Regulus.Utility.IStage.Enter()
        {
            _User.VerifyProvider.Supply += VerifyProvider_Supply;
        }

        void VerifyProvider_Supply(Regulus.Project.SamebestKeys.IVerify obj)
        {
            var val = obj.Login(account , "1");
            val.OnValue += ResultEvent;            
        }

        void Regulus.Utility.IStage.Leave()
        {
            _User.VerifyProvider.Supply -= VerifyProvider_Supply;
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }
    }
}
