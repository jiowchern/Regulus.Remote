using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VGameWebApplication.Storage.Handler
{
    public class FindAccount : WaitSomething<VGame.Project.FishHunter.Data.Account>
    {
        private VGame.Project.FishHunter.IAccountFinder accountFinder;
        
        VGame.Project.FishHunter.Data.Account _Result;
        private Guid accountId;
        

        

        public FindAccount(VGame.Project.FishHunter.IAccountFinder accountFinder, Guid accountId)
        {
            // TODO: Complete member initialization
            this.accountFinder = accountFinder;
            this.accountId = accountId;
        }

        protected override void Start()
        {
            accountFinder.FindAccountById(accountId).OnValue += _ResultAccount;
        }

        private void _ResultAccount(VGame.Project.FishHunter.Data.Account obj)
        {
            _Result = obj;
            Stop();
        }

        protected override VGame.Project.FishHunter.Data.Account End()
        {
            return _Result;
        }
    }
}