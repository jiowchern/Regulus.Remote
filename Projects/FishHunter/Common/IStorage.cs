using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public interface IStorage : IAccountFinder, IAccountManager , IRecordQueriers, ITradeAccount
    {

    }
}
