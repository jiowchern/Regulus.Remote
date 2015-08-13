
using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Storage
{
	public interface IUser : IUpdatable
	{
		Regulus.Remoting.User Remoting { get; }

		INotifier<IVerify> VerifyProvider { get; }

		INotifier<IStorageCompetences> StorageCompetencesProvider { get; }

		

		INotifier<T> QueryProvider<T>();
	}
}
