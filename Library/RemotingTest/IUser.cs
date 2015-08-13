using Regulus.Remoting;
using Regulus.Utility;

namespace RemotingTest
{
	public interface IUser : IUpdatable
	{
		Regulus.Remoting.User Remoting { get; }

		INotifier<ITestReturn> TestReturnProvider { get; }
	}
}
