using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComplexApplication
{
	class Hall 
	{		
		Samebest.Game.FrameworkRoot	_Users = new Samebest.Game.FrameworkRoot();
		

		internal bool Update()
		{
			_Users.Update();
			return true;
		}





		internal Regulus.Project.Crystal.User CreateUser(Samebest.Remoting.Soul.SoulProvider provider)
		{
			var user = new Regulus.Project.Crystal.User(provider);
			_Users.AddFramework(user);
			user.InactiveEvent += () => { _Users.RemoveFramework(user); };
			return user;
		}
	}
}
