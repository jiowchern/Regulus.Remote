using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public interface IAccountManager : IAccountCreator
    {
        Regulus.Remoting.Value<Data.Account[]> QueryAllAccount();
        
        Regulus.Remoting.Value<ACCOUNT_REQUEST_RESULT> Delete(string account);
        Regulus.Remoting.Value<ACCOUNT_REQUEST_RESULT> Update(Data.Account account);        
    }
}
