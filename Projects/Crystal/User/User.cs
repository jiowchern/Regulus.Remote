using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.Crystal
{
	public interface IUser : Regulus.Game.IFramework
	{
		Regulus.Remoting.Ghost.IProviderNotice<IVerify> VerifyProvider { get ; }		
	}
}
