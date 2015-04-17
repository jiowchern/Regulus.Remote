using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public class DummyStorage : IAccountFinder
    {
        Regulus.Remoting.Value<Data.Account> IAccountFinder.FindAccountByName(string id)
        {
            return new Data.Account { Id = Guid.NewGuid(), Password = "pw", Name = id, Competnce = Data.Account.COMPETENCE.ALL };
        }


        Regulus.Remoting.Value<Data.Account> IAccountFinder.FindAccountById(Guid accountId)
        {
            return new Data.Account { Id = accountId, Password = "pw", Name = "name", Competnce = Data.Account.COMPETENCE.ALL };
        }
    }
}
