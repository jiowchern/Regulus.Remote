using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    class VerifyStage :Regulus.Utility.IStage
    {
        private Regulus.Remoting.Ghost.IProviderNotice<IVerify> _Provider;
        private string _Account;
        private string _Password;

        public delegate void DoneCallback();
        public event DoneCallback SuccessEvent;
        public event DoneCallback FailEvent;

        public VerifyStage(Regulus.Remoting.Ghost.IProviderNotice<IVerify> provider, string account, string password)
        {
            
            this._Provider = provider;
            this._Account = account;
            this._Password = password;
        }

        void Regulus.Utility.IStage.Enter()
        {
            _Provider.Supply += _Provider_Supply;
        }

        void _Provider_Supply(IVerify obj)
        {
            obj.Login(_Account , _Password).OnValue += _Result;
        }

        private void _Result(bool obj)
        {
            if (obj)
                SuccessEvent();
            else
                FailEvent();
        }

        void Regulus.Utility.IStage.Leave()
        {
            
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }
    }
}
