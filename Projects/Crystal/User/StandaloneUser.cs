using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.Crystal.Standalone
{	
	using Regulus.Project.Crystal.Game;
	public class User : IUser
	{		
		Regulus.Project.Crystal.Game.Hall _Hall;
		Regulus.Standalong.Agent _Agent ;
		public User()
		{
						
		}

		Regulus.Remoting.Ghost.IProviderNotice<IVerify> IUser.VerifyProvider
		{
			get { return _Agent.QueryProvider<IVerify>(); }
		}

		public void Launch()
		{
			_Agent = new Standalong.Agent();
			_Agent.Launch();
			_Hall = new Regulus.Project.Crystal.Game.Hall();
			_Hall.CreateUser(_Agent , new Regulus.Project.Crystal.Standalone.Storage());
		}

		public bool Update()
		{
			_Agent.Update();
			_Hall.Update(); 
			return true;
		}

		public void Shutdown()
		{
			_Agent.Shutdown();			
		}
	}
}
