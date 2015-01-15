using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus;

namespace VGame.Project.FishHunter
{
    using Regulus.Extension;
    public class CommandParser : Regulus.Framework.ICommandParsable<IUser>
    {
    
        Regulus.Remoting.CommandAutoBuild _CommandAutoBuild;
        private Regulus.Utility.Command _Command;
        private Regulus.Utility.Console.IViewer _View;
        private IUser _User;

        public CommandParser(Regulus.Utility.Command command, Regulus.Utility.Console.IViewer view, IUser user)
        {            
            this._Command = command;
            this._View = view;
            this._User = user;
            _CommandAutoBuild = new Regulus.Remoting.CommandAutoBuild(_Command);
        }


        void Regulus.Framework.ICommandParsable<IUser>.Setup()
        {
            var gpiBinder = _CommandAutoBuild.Add<Regulus.Game.IConnect>(_User.Remoting.ConnectProvider);
            gpiBinder.Add("Connect", (connect) => { return new Regulus.Remoting.CommandParamBuilder().BuildRemoting<string, int, bool>(connect.Connect, _ConnectResult); });
            
        }

        void Regulus.Framework.ICommandParsable<IUser>.Clear()
        {
            _CommandAutoBuild.Remove(_User.Remoting.OnlineProvider);
        }
        
        private void _ConnectResult(bool result)
        {
            _View.WriteLine(string.Format("Connect result {0}", result));

        }
    }
}
