using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGame.Project.FishHunter.Storage
{
    public class StandalongFactory : Regulus.Framework.IUserFactoty<IUser>
    {
        Regulus.Remoting.ICore _Core; 
        public StandalongFactory(Regulus.Remoting.ICore core)
        {
            _Core = core;
        }
        
        IUser Regulus.Framework.IUserFactoty<IUser>.SpawnUser()
        {
      
            var agent = new Regulus.Standalong.Agent();
            agent.ConnectedEvent += () => { _Core.AssignBinder(agent); };            
            return new User(agent);
        }

        Regulus.Framework.ICommandParsable<IUser> Regulus.Framework.IUserFactoty<IUser>.SpawnParser(Regulus.Utility.Command command, Regulus.Utility.Console.IViewer view, IUser user)
        {
            return new CommandParser(command, view, user);
        }
    }
}
