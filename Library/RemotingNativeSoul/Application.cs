using System;
using System.Collections.Generic;

namespace Regulus.Remoting.Soul.Native
{
	public interface IUser : Regulus.Utility.IUpdatable
	{

	}


	public class Application : Regulus.Game.ConsoleFramework<IUser>
	{
		
		public Application(Regulus.Utility.Console.IViewer view, Regulus.Utility.Console.IInput input)
			: base(view, input)
		{
			
		}
		protected override Regulus.Game.ConsoleFramework<IUser>.ControllerProvider[] _ControllerProvider()
		{
			return new Application.ControllerProvider[] 
            {
                new Application.ControllerProvider { Command = "stand" , Spawn =  _BuildStandController},                
            };
		}

		private IController _BuildStandController()
		{
			return new Controller(Command , _Viewer);
		}
		
	}
}