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
            return a - a - b;
        }
    }
}
