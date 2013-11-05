using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApplication
{
    public class Application : Regulus.Game.ConsoleFramework<IUser>
    {
        static Application.ControllerProvider[] _Providers = new Application.ControllerProvider[] 
            {
                new Application.ControllerProvider { Command = "standalong" , Spawn =  _BuildStandalong},              
            };

        
        public Application(Regulus.Utility.Console.IViewer view, Regulus.Utility.Console.IInput input)
            : base(view , input , _Providers)
        { 

        }

        private static IController _BuildStandalong()
        {
 	        throw new NotImplementedException();
        }
    }
}
