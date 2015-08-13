using Regulus.Remoting;
using Regulus.Utility;


using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Formula
{
	public interface IUser : IUpdatable
	{
		Regulus.Remoting.User Remoting { get; }

		INotifier<IVerify> VerifyProvider { get; }

		INotifier<IFishStageQueryer> FishStageQueryerProvider { get; }


	}
}
