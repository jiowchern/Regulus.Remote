using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemotingTest
{
    class Server : Regulus.Utility.ICore, ITestReturn
    {
        void Regulus.Utility.ICore.ObtainController(Regulus.Remoting.ISoulBinder binder)
        {
            binder.Return<ITestReturn>(this);
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
    }
}
