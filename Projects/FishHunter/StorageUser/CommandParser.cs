using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Storage
{
    class CommandParser : Regulus.Framework.ICommandParsable<IUser>
    {
        private Regulus.Utility.Command command;
        private Regulus.Utility.Console.IViewer view;
        private IUser user;

        public CommandParser(Regulus.Utility.Command command, Regulus.Utility.Console.IViewer view, IUser user)
        {
            // TODO: Complete member initialization
            this.command = command;
            this.view = view;
            this.user = user;
        }

        void Regulus.Framework.ICommandParsable<IUser>.Setup(Regulus.Remoting.IGPIBinderFactory build)
        {
            
        }

        void Regulus.Framework.ICommandParsable<IUser>.Clear()
        {
            
        }
    }
}
