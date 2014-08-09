using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApplication
{
    public class Application : Regulus.Game.Framework<IUser>
    {
        
        public Application(Regulus.Utility.Console.IViewer view, Regulus.Utility.Console.IInput input)
            : base(view , input )
        { 

        }

        private static IController _BuildStandalong()
        {
 	        throw new NotImplementedException();
        }

        protected override Regulus.Game.Framework<IUser>.ControllerProvider[] _ControllerProvider()
        {
            return new Application.ControllerProvider[] 
            {
                new Application.ControllerProvider { Command = "standalong" , Spawn =  _BuildStandalong},              
            };
        }
    }
}
