using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemotingTest
{
    public interface ITestInterface
    {
        Regulus.Remoting.Value<int> Add(int a , int b);
        event Action<int> ReturnEvent;
    }
    public interface ITestReturn 
    {
        
        Regulus.Remoting.Value<ITestInterface> Test(int a, int b);
    }
}
