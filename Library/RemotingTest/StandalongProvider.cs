using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemotingTest
{
    public class StandalongProvider : Regulus.Framework.IUserFactoty<IUser>
    {
        Regulus.Remoting.ICore _Standalong;
        public StandalongProvider(Regulus.Remoting.ICore core)
        {
            _Standalong = core;
        }
        IUser Regulus.Framework.IUserFactoty<IUser>.SpawnUser()
        {
            var agent = new Regulus.Standalong.Agent();
            agent.ConnectedEvent += () => { _Standalong.AssignBinder(agent); };
            return new User(agent);
        }

        Regulus.Framework.ICommandParsable<IUser> Regulus.Framework.IUserFactoty<IUser>.SpawnParser(Regulus.Utility.Command command, Regulus.Utility.Console.IViewer view, IUser user)
        {
            return new CommandParser();
        }
    }
}
