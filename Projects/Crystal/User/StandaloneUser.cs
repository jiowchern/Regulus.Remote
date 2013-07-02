using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.Crystal.Standalone
{	
	public class User : IUser
	{
		Regulus.Remoting.Standalone.Provider<IVerify> _VerifyProvider;
		Core	_Core;
		Regulus.Standalong.GhostProvider _GhostProvider = new Regulus.Standalong.GhostProvider();
		public User()
		{
			_VerifyProvider = new Regulus.Remoting.Standalone.Provider<IVerify>();									
		}

		Samebest.Remoting.Ghost.IProviderNotice<IVerify> IUser.VerifyProvider
		{
			get { return _VerifyProvider; }
		}

		public void Launch()
		{
			_Core = new Core(_GhostProvider);
		}

		public bool Update()
		{
			_Core.Update();
			return true;
		}

		public void Shutdown()
		{
		
		}
	}
}
