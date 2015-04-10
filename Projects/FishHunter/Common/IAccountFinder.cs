using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    
    public interface IAccountFinder
    {
        Regulus.Remoting.Value<Data.Account> FindAccount(string id);
    }
}
