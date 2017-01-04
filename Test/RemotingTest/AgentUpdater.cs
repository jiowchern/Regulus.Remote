using Regulus.Remoting;

namespace RemotingTest
{
	internal class AgentUpdater
	{
		private readonly IAgent _Agent;

		public AgentUpdater(IAgent agent)
		{
			_Agent = agent;
		}

		internal void Run()
		{
			_Agent.Launch();

			var enable = true;
			_Agent.BreakEvent += () => enable = false;
			while(enable)
			{
				_Agent.Update();
			}

			_Agent.Shutdown();
		}
	}
}
