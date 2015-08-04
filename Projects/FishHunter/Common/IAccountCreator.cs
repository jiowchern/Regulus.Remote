using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Common.GPIs
{
    public interface IAccountCreator
    {
        Regulus.Remoting.Value<VGame.Project.FishHunter.Common.GPIs.ACCOUNT_REQUEST_RESULT> Create(VGame.Project.FishHunter.Common.Datas.Account account);
    }
}
