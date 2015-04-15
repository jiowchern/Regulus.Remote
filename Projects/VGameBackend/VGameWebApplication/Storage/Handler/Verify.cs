using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGameWebApplication.Storage.Handler
{
    class Verify : WaitSomething<bool>
    {
        private VGame.Project.FishHunter.Storage.IUser user;
        private string account;
        private string password;
        bool _Result;
        public Verify(VGame.Project.FishHunter.Storage.IUser user, string account, string password)
        {
            // TODO: Complete member initialization
            this.user = user;
            this.account = account;
            this.password = password;
        }

        protected override void Start()
        {
            user.VerifyProvider.Supply += VerifyProvider_Supply;
        }

        void VerifyProvider_Supply(VGame.Project.FishHunter.IVerify obj)
        {
            var result = obj.Login(account, password);
            result.OnValue += result_OnValue;
        }

        void result_OnValue(bool obj)
        {
            _Result = obj;
            Stop();
        }

        protected override bool End()
        {
            return _Result;
        }

        public bool Result { get { return _Result; } }
    }
}
