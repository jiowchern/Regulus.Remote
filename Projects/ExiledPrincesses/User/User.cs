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
        Regulus.Remoting.Ghost.IProviderNotice<ITown> TownProvider { get; }	
        Regulus.Remoting.Ghost.IProviderNotice<IAdventure> AdventureProvider { get; }

        Regulus.Remoting.Ghost.IProviderNotice<IAdventureIdle> AdventureIdleProvider { get; }
        Regulus.Remoting.Ghost.IProviderNotice<IAdventureGo> AdventureGoProvider { get; }
        Regulus.Remoting.Ghost.IProviderNotice<IAdventureChoice> AdventureChoiceProvider { get; }

        Regulus.Remoting.Ghost.IProviderNotice<IActor> ActorProvider { get; }
    }

    
}
