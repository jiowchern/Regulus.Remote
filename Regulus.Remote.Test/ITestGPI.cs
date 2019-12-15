using Regulus.Remote;

namespace RemotingTest
{
	public interface ITestGPI
	{
		Value<int> Add(int a, int b);
	}
}
