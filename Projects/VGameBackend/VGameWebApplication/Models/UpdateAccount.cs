using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VGameWebApplication.Models 
{
    public class UpdateAccount     
    {
        public UpdateAccount()
        {
            
        }
        public VGame.Project.FishHunter.Data.Account TheAccount {get ; set;}

        public bool AccountFinder {
            get { return TheAccount.Competnce.HasFlag(VGame.Project.FishHunter.Data.Account.COMPETENCE.ACCOUNT_FINDER); }
           // set { return TheAccount.Competnce |== value ? VGame.Project.FishHunter.Data.Account.COMPETENCE.ACCOUNT_FINDER); }
        }
        

    }
}