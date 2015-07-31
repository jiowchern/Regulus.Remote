// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Server.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Server type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;

#endregion

namespace RemotingTest
{
	internal class Server : ICore, ITestReturn, ITestGPI
	{
		private ISoulBinder _Binder;

		void ICore.AssignBinder(ISoulBinder binder)
		{
			binder.Return<ITestReturn>(this);
			_Binder = binder;
			_Binder.Bind<ITestGPI>(this);
		}

		bool IUpdatable.Update()
		{
			return true;
		}

		void IBootable.Launch()
		{
		}

		void IBootable.Shutdown()
		{
		}

		Value<int> ITestGPI.Add(int a, int b)
		{
			return a + b;
		}

		Value<ITestInterface> ITestReturn.Test(int a, int b)
		{
			return new TestInterface();
		}
	}
}