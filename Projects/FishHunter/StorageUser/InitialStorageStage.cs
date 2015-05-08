using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public class VerifyStorageStage : Regulus.Utility.IStage
    {
        public delegate void DoneCallback(bool result);
        public event DoneCallback DoneEvent;
        Storage.IUser _User;
        private string _Account;
        private string _Password;
        public VerifyStorageStage(Storage.IUser user , string account , string password)
        {
            _Account = account;
            _Password = password;
            _User = user;
        }
        void Regulus.Utility.IStage.Update()
        {

        }

        void Regulus.Utility.IStage.Leave()
        {
            _User.VerifyProvider.Supply -= _ToVerify;
        }

        

        void Regulus.Utility.IStage.Enter()
        {
            _User.VerifyProvider.Supply += _ToVerify;
        }

        private void _ToVerify(IVerify obj)
        {
            var result = obj.Login(_Account, _Password);
            result.OnValue += (val) => { DoneEvent(val); };
        }

        
        
    }
}
