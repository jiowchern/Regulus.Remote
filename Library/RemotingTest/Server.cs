using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemotingTest
{
    class Server : Regulus.Utility.ICore, ITestReturn, ITestGPI
    {
        Regulus.Remoting.ISoulBinder _Binder;
        void Regulus.Utility.ICore.ObtainController(Regulus.Remoting.ISoulBinder binder)
        {
            binder.Return<ITestReturn>(this);
            _Binder = binder;
            _Binder.Bind<ITestGPI>(this);
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {            
        }

        Regulus.Remoting.Value<ITestInterface> ITestReturn.Test(int a, int b)
        {
            return new TestInterface();
        }



        Regulus.Remoting.Value<int> ITestGPI.Add(int a, int b)
        {
            return a + b;
        }
    }
}
