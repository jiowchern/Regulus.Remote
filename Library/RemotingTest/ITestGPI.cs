using Regulus.Remoting;

namespace RemotingTest
{
	public interface ITestGPI
	{
		Value<int> Add(int a, int b);
	}
}
