using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
	public class Hall 
	{		
		Regulus.Game.FrameworkRoot	_Users = new Regulus.Game.FrameworkRoot();

		public bool Update()
		{
			_Users.Update();
			return true;
		}

		public Regulus.Project.ExiledPrincesses.Game.Core CreateUser(Regulus.Remoting.ISoulBinder binder, IStorage storage, IMap zone )
		{
            var core = new Regulus.Project.ExiledPrincesses.Game.Core(binder, storage, zone);
			_Users.AddFramework(core);
			core.InactiveEvent += () => { _Users.RemoveFramework(core); };
			return core;
		}
	}
}
