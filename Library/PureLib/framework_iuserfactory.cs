using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Framework
{
    public interface IUserFactoty<TUser>
    {
        TUser SpawnUser();
        ICommandParsable<TUser> SpawnParser(Regulus.Utility.Command command , Regulus.Utility.Console.IViewer view , TUser user);
    }
}
