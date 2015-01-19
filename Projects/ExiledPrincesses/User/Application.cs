using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.ExiledPrincesses
{
    public class Application : Regulus.Utility.Framework<Regulus.Project.ExiledPrincesses.IUser> 
    {

        private IController _BuildRemoting()
        {
            IController uc = new Remoting.UserController(_Viewer, Command);            
            return uc;
        }

        private IController _BuildStandalong()
        {
            return new Standalong.UserController(_Viewer, Command);
        }

        public Application(Regulus.Utility.Console.IViewer view, Regulus.Utility.Console.IInput input)
            : base(view, input)
        {
        }

        protected override Regulus.Utility.Framework<IUser>.ControllerProvider[] _ControllerProvider()
        {
            return new Application.ControllerProvider[] 
            {
                new Application.ControllerProvider { Command = "standalong" , Spawn =  _BuildStandalong},
                new Application.ControllerProvider { Command = "remoting" , Spawn = _BuildRemoting}
            };
        }
    }
}
