using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSoul
{
	public class TestApplication : Samebest.Remoting.Soul.PhotonApplication, Samebest.Remoting.PhotonExpansion.IPhotonFramework
	{
		

		protected override Samebest.Remoting.PhotonExpansion.IPhotonFramework _Setup()
		{
			return this;
		}



		void Samebest.Remoting.PhotonExpansion.IPhotonFramework.ObtainController(Samebest.Remoting.Soul.SoulProvider provider)
		{
			var soul = new CTestSoul();
			provider.Bind<TestRemotingCommon.ITest>(soul);
			//provider.Unbind<TestRemotingCommon.ITest>(soul);
		}

		void Samebest.Game.IFramework.Launch()
		{
		//	throw new NotImplementedException();
		}

		bool Samebest.Game.IFramework.Update()
		{
		//	throw new NotImplementedException();
			return true;
		}

		void Samebest.Game.IFramework.Shutdown()
		{
		//	throw new NotImplementedException();
		}
	}
}
