using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.Crystal.Standalone
{	
	using Regulus.Project.Crystal.Game;
	public class User : IUser
	{
		Regulus.Remoting.Standalone.Provider<IVerify> _VerifyProvider;		
		Regulus.Project.Crystal.Game.Hall _Hall;
		Regulus.Standalong.Provider _Provider = new Regulus.Standalong.Provider();
		public User()
		{
			_VerifyProvider = new Regulus.Remoting.Standalone.Provider<IVerify>();

			
		}

		Regulus.Remoting.Ghost.IProviderNotice<IVerify> IUser.VerifyProvider
		{
			get { return _VerifyProvider; }
		}

		public void Launch()
		{
			//_Provider.Register(_VerifyProvider);
			
			_Hall = new Regulus.Project.Crystal.Game.Hall();
			_Hall.CreateUser(_Provider , new Regulus.Project.Crystal.Standalone.Storage());
		}

		public bool Update()
		{
			_Hall.Update(); 
			return true;
		}

		public void Shutdown()
		{
			//_Provider.Unregister(_VerifyProvider);
		}
	}
}
