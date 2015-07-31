// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITestGPI.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ITestGPI type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Remoting;

#endregion

namespace RemotingTest
{
	public interface ITestGPI
	{
		Value<int> Add(int a, int b);
	}
}