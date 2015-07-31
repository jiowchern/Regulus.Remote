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

#endregion

namespace VGame.Project.FishHunter
{
	public interface IUser : IUpdatable
	{
		Regulus.Remoting.User Remoting { get; }

		INotifier<IVerify> VerifyProvider { get; }

		INotifier<IPlayer> PlayerProvider { get; }

		INotifier<ILevelSelector> LevelSelectorProvider { get; }

		INotifier<IAccountStatus> AccountStatusProvider { get; }
	}
}