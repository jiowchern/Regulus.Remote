using Regulus.Remoting;
using Regulus.Utility;


using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPI;

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
