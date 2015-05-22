using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemotingTest
{
    public interface ITestGPI
    {

        Regulus.Remoting.Value<int> Add(int a,int b);
    }
}
