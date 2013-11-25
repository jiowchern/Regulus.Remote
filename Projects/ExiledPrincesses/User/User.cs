using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.ExiledPrincesses
{
	public interface IUser : Regulus.Game.IFramework
	{
		Regulus.Remoting.Ghost.IProviderNotice<IVerify> VerifyProvider { get ; }
        Regulus.Remoting.Ghost.IProviderNotice<IUserStatus> StatusProvider { get; }
        Regulus.Remoting.Ghost.IProviderNotice<IParking> ParkingProvider { get; }	
        Regulus.Remoting.Ghost.IProviderNotice<IAdventure> AdventureProvider { get; }
        

        
	    
        
	}

    
}
