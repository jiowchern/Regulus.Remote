using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComplexApplication;

namespace Regulus.Project.Crystal
{
	class ComplexFramework : Samebest.Remoting.PhotonExpansion.IPhotonFramework
	{
		Hall	_Hall;
		Storage _Stroage;
		void Samebest.Remoting.PhotonExpansion.IPhotonFramework.ObtainController(Samebest.Remoting.Soul.SoulProvider provider)
		{

			var user = _Hall.CreateUser(provider);

		}

		void Samebest.Game.IFramework.Launch()
		{
			_Stroage = new Storage();
			_Stroage.Initial();

			_Hall = new Hall();			
		}

		bool Samebest.Game.IFramework.Update()
		{
			_Hall.Update();
			return true;
		}

		void Samebest.Game.IFramework.Shutdown()
		{			
			_Stroage.Finial();
		}
	}
}
