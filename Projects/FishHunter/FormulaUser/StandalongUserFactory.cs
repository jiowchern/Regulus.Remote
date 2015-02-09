using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Formula
{
    public class StandalongUserFactory 
        :Regulus.Framework.IUserFactoty<IUser>
    {
        Regulus.Utility.ICore _Standalong;
        public StandalongUserFactory(Regulus.Utility.ICore core)
        {
            _Standalong = core;

            if (_Standalong == null)
                throw new ArgumentNullException("Core is null");
        }
        IUser Regulus.Framework.IUserFactoty<IUser>.SpawnUser()
        {
            var agent = new Regulus.Standalong.Agent();
            agent.ConnectedEvent += () => { _Standalong.ObtainController(agent); };
            
            return new User(agent);
        }

        Regulus.Framework.ICommandParsable<IUser> Regulus.Framework.IUserFactoty<IUser>.SpawnParser(Regulus.Utility.Command command, Regulus.Utility.Console.IViewer view, IUser user)
        {
            return new CommandParser(command , view , user);
        }
    }
}
