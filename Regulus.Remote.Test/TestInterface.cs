using System;


using Regulus.Remote;

namespace RemotingTest
{
	internal class TestInterface : ITestInterface
	{
		private event Action<int> _ReturnEvent;

		Value<int> ITestInterface.Add(int a, int b)
		{
			var v = a - a - b;
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
