using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemotingTest
{
    public interface ITestReturn
    {
        Regulus.Remoting.Value<int> Test(int a , int b);
    }
}
