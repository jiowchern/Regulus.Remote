// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestInterface.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the TestInterface type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Regulus.Remoting;

#endregion

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