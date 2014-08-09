using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TestNativeUser
{
    public class Application : Regulus.Game.Framework<TestNativeUser.IUser>
    {
        Regulus.Utility.Console.IViewer _View;
        public Application(Regulus.Utility.Console.IViewer view, Regulus.Utility.Console.IInput input)
            : base(view, input)
        {
            _View = view;
        }

        protected override Regulus.Game.Framework<IUser>.ControllerProvider[] _ControllerProvider()
        {
            return new Application.ControllerProvider[] 
            {                
                new Application.ControllerProvider { Command = "remoting" , Spawn = _BuildRemoting}
            };
        }

        private IController _BuildRemoting()
        {
            return new RemotingController(Command, _View);
        }
    }
}
