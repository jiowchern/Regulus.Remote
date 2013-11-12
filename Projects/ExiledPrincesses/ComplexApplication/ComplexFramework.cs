using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.ExiledPrincesses
{
	class ComplexFramework : Regulus.Remoting.PhotonExpansion.IPhotonFramework
	{
        Game.Zone _World;
		
		Storage _Stroage;
		void Regulus.Remoting.PhotonExpansion.IPhotonFramework.ObtainController(Regulus.Remoting.Soul.SoulProvider provider)
		{
            _World.Enter(provider);
		}

		void Regulus.Game.IFramework.Launch()
		{
			_Stroage = new Storage();
			_Stroage.Initial();

            _World = new Game.Zone(_Stroage);
			
		}

		bool Regulus.Game.IFramework.Update()
		{
            _World.Update();
			return true;
		}

		void Regulus.Game.IFramework.Shutdown()
		{			
			_Stroage.Finial();			
		}
	}
}
