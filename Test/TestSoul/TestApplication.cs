using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSoul
{
	public class TestApplication : Regulus.Remoting.Soul.PhotonApplication, Regulus.Remoting.PhotonExpansion.IPhotonFramework
	{
		

		protected override Regulus.Remoting.PhotonExpansion.IPhotonFramework _Setup()
		{
			return this;
		}



		void Regulus.Remoting.PhotonExpansion.IPhotonFramework.ObtainController(Regulus.Remoting.Soul.SoulProvider provider)
		{
			var soul = new CTestSoul();
			provider.Bind<TestRemotingCommon.ITest>(soul);
            soul.invoke();  
			//provider.Unbind<TestRemotingCommon.ITest>(soul);
		}



		bool Regulus.Utility.IUpdatable.Update()
		{
			throw new NotImplementedException();
		}

		void Regulus.Framework.ILaunched.Launch()
		{
			throw new NotImplementedException();
		}

		void Regulus.Framework.ILaunched.Shutdown()
		{
			throw new NotImplementedException();
		}
	}
}
