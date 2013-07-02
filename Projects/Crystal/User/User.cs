using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.Crystal
{
	public interface IUser : Samebest.Game.IFramework
	{
		Samebest.Remoting.Ghost.IProviderNotice<IVerify> VerifyProvider { get ; }		
	}
}
