using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.Crystal
{
	class ComplexFramework : Regulus.Remoting.PhotonExpansion.IPhotonFramework
	{
		Regulus.Project.Crystal.Game.Hall	_Hall;
		Storage _Stroage;
		void Regulus.Remoting.PhotonExpansion.IPhotonFramework.ObtainController(Regulus.Remoting.Soul.SoulProvider provider)
		{

            var user = _Hall.CreateUser(provider, _Stroage);

		}

		void Regulus.Game.IFramework.Launch()
		{
			_Stroage = new Storage();
			_Stroage.Initial();

			_Hall = new Regulus.Project.Crystal.Game.Hall();		
		}

		bool Regulus.Game.IFramework.Update()
		{
			_Hall.Update();
			return true;
		}

		void Regulus.Game.IFramework.Shutdown()
		{			
			_Stroage.Finial();
		}
	}
}
