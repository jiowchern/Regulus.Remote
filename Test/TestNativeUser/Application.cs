using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNativeUser
{
    public class Application : Regulus.Game.ConsoleFramework<TestNativeUser.IUser>
    {

        public Application(Regulus.Utility.Console.IViewer view, Regulus.Utility.Console.IInput input)
            : base(view, input)
        {
        }

        protected override Regulus.Game.ConsoleFramework<IUser>.ControllerProvider[] _ControllerProvider()
        {
            return new Application.ControllerProvider[] 
            {                
                new Application.ControllerProvider { Command = "remoting" , Spawn = _BuildRemoting}
            };
        }

        private IController _BuildRemoting()
        {
            return new RemotingController(Command);
        }
    }
}
