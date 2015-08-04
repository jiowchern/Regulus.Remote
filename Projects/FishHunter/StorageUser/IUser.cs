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

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPIs;

#endregion

namespace VGame.Project.FishHunter.Storage
{
	public interface IUser : IUpdatable
	{
		Regulus.Remoting.User Remoting { get; }

		INotifier<IVerify> VerifyProvider { get; }

		INotifier<IStorageCompetences> StorageCompetncesProvider { get; }

		INotifier<T> QueryProvider<T>();
	}
}