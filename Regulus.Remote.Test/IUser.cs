using Regulus.Remote;
using Regulus.Utility;

namespace RemotingTest
{
	public interface IUser : IUpdatable
	{
		Regulus.Remote.User Remoting { get; }

		INotifier<ITestReturn> TestReturnProvider { get; }
	}
}
