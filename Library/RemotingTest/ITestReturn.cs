// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITestReturn.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ITestInterface type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Regulus.Remoting;

#endregion

namespace RemotingTest
{
	public interface ITestInterface
	{
		event Action<int> ReturnEvent;

		Value<int> Add(int a, int b);
	}

	public interface ITestReturn
	{
		Value<ITestInterface> Test(int a, int b);
	}
}