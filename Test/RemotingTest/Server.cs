using Regulus.Framework;
using Regulus.Remote;
using Regulus.Utility;

namespace RemotingTest
{
	internal class Server : ICore, ITestReturn, ITestGPI
	{
		private ISoulBinder _Binder;

		void IBinderProvider.AssignBinder(ISoulBinder binder)
		{
			binder.Return<ITestReturn>(this);
			_Binder = binder;
			_Binder.Bind<ITestGPI>(this);
		}

		

		void ICore.Launch(IProtocol protocol , ICommand command)
		{
		}

		void ICore.Shutdown()
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
