using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    public class Console : Regulus.Game.ConsoleFramework<Regulus.Project.SamebestKeys.IUser> 
    {
        private IController _BuildRemoting()
        {            
            return new Remoting.UserController(_Viewer, Command);
        }

        /*private IController _BuildStandalong()
        {
            return new Standalong.UserController(_Viewer, Command);
        }*/

        public Console(Regulus.Utility.Console.IViewer view, Regulus.Utility.Console.IInput input)
            : base(view, input)
        {
        }

        protected override Regulus.Game.ConsoleFramework<Regulus.Project.SamebestKeys.IUser>.ControllerProvider[] _ControllerProvider()
        {
            return new Console.ControllerProvider[] 
            {
                //new Console.ControllerProvider { Command = "standalong" , Spawn =  _BuildStandalong},
                new Console.ControllerProvider { Command = "remoting" , Spawn = _BuildRemoting}
            };
        }
    }
}
