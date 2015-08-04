using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Common.GPIs
{
    public interface IAccountCreator
    {
        Regulus.Remoting.Value<Common.Datas.ACCOUNT_REQUEST_RESULT> Create(Common.Datas.Account account);
    }
}
