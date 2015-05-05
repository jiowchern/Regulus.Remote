using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemotingTest
{
    class TestInterface : ITestInterface
    {
        Regulus.Remoting.Value<int> ITestInterface.Add(int a, int b)
        {
            var v = a - a - b;
            _ReturnEvent(v);
            return v;
        }




        event Action<int> _ReturnEvent;
        event Action<int> ITestInterface.ReturnEvent
        {
            add { _ReturnEvent += value; }
            remove { _ReturnEvent -= value; }
        }
    }
}
