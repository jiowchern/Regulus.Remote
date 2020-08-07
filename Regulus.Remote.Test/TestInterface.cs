using Regulus.Remote;
using System;

namespace RemotingTest
{
    internal class TestInterface : ITestInterface
    {
        private event Action<int> _ReturnEvent;

        Value<int> ITestInterface.Add(int a, int b)
        {
            int v = a - a - b;
            _ReturnEvent(v);
            return v;
        }

        event Action<int> ITestInterface.ReturnEvent
        {
            add { _ReturnEvent += value; }
            remove { _ReturnEvent -= value; }
        }
    }
}
