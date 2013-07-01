using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.Crystal.Standalone
{	
	public class User : IUser
	{
		Regulus.Remoting.Standalone.Provider<IVerify> _VerifyProvider;
		public User()
		{
			_VerifyProvider = new Regulus.Remoting.Standalone.Provider<IVerify>();									
		}

		Samebest.Remoting.Ghost.IProviderNotice<IVerify> IUser.VerifyProvider
		{
			get { return _VerifyProvider; }
		}

		void IUser.Update()
		{
			
		}
	}
}
