using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Formula
{
    public class RemotingUserFactory : Regulus.Framework.IUserFactoty<IUser>
    {
        IUser Regulus.Framework.IUserFactoty<IUser>.SpawnUser()
        {
            return new User(Regulus.Remoting.Ghost.Native.Agent.Create());
        }

        Regulus.Framework.ICommandParsable<IUser> Regulus.Framework.IUserFactoty<IUser>.SpawnParser(Regulus.Utility.Command command, Regulus.Utility.Console.IViewer view, IUser user)
        {
            return new CommandParser(command, view, user);
        }
    }
}
