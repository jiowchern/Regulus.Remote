using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.Crystal.Game
{
	public class Hall 
	{		
		Regulus.Game.FrameworkRoot	_Users = new Regulus.Game.FrameworkRoot();

		public bool Update()
		{
			_Users.Update();
			return true;
		}

		public Regulus.Project.Crystal.Game.Core CreateUser(Regulus.Remoting.ISoulBinder binder, IStorage storage)
		{
			var core = new Regulus.Project.Crystal.Game.Core(binder, storage);
			_Users.AddFramework(core);
			core.InactiveEvent += () => { _Users.RemoveFramework(core); };
			return core;
		}
	}
}
