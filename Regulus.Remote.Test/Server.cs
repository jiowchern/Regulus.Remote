using Regulus.Remote;
using Regulus.Utility;


namespace RemotingTest
{
    internal class Server : IEntry, ITestReturn, ITestGPI
    {
        private IBinder _Binder;

        void IBinderProvider.AssignBinder(IBinder binder)
        {
            binder.Return<ITestReturn>(this);
            _Binder = binder;
            _Binder.Bind<ITestGPI>(this);
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
