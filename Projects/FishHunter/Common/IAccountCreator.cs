using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public interface IAccountCreator
    {
        Regulus.Remoting.Value<ACCOUNT_REQUEST_RESULT> Create(Data.Account account);
    }
}
