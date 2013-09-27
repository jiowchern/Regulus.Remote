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

    public static class UserGenerator
    {
        static public IUser BuildStandalong()
        {
            return new Regulus.Project.Crystal.Standalone.User() ;
        }
        static public IUser BuildRemoting()
        {
            return new Regulus.Project.Crystal.ConnectUser();
        }
    }
}
