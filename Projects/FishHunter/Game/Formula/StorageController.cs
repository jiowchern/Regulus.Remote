using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Formula
{
    public struct StorageController
    {
        public StorageController(IAccountFinder account_finder)
        {
            _AccountFinder = account_finder;
        }
        IAccountFinder _AccountFinder;
        public IAccountFinder AccountFinder { get { return _AccountFinder; } }
    }
}
