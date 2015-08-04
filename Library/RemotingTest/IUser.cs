// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUser.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IUser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Remoting;
using Regulus.Utility;

#endregion

namespace RemotingTest
{
	public interface IUser : IUpdatable
	{
		Regulus.Remoting.User Remoting { get; }

		INotifier<ITestReturn> TestReturnProvider { get; }
	}
}