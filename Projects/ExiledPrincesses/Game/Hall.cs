using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    public delegate void OnNewUser(Guid account);
	public class Hall 
	{		

		Regulus.Game.FrameworkRoot	_Users = new Regulus.Game.FrameworkRoot();
        
        public event OnNewUser NewUserEvent;
        public Hall()
        {
            NewUserEvent = (id) => { };
        }
		public bool Update()
		{
			_Users.Update();
			return true;
		}

		public Regulus.Project.ExiledPrincesses.Game.Core CreateUser(Regulus.Remoting.ISoulBinder binder, IStorage storage, IZone zone )
		{
            var core = new Regulus.Project.ExiledPrincesses.Game.Core(binder, storage, zone);
            core.VerifySuccessEvent += (id )=>
            {
                NewUserEvent(id);
                NewUserEvent += core.OnKick;
            };
            


			_Users.AddFramework(core);
			core.InactiveEvent += () => 
            {
                NewUserEvent -= core.OnKick;
                _Users.RemoveFramework(core); 
            };
			return core;
		}
	}
}
