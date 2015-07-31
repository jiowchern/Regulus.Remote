// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentUpdater.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the AgentUpdater type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Remoting;

#endregion

namespace RemotingTest
{
	internal class AgentUpdater
	{
		private readonly IAgent _Agent;

		public AgentUpdater(IAgent agent)
		{
			this._Agent = agent;
		}

		internal void Run()
		{
			_Agent.Launch();

			var enable = true;
			_Agent.BreakEvent += () => enable = false;
			while (enable)
			{
				_Agent.Update();
			}

			_Agent.Shutdown();
		}
	}
}