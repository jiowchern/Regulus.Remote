using Regulus.Utility;

namespace Regulus.Remote.Ghost
{
	public partial class Agent
	{
		private class TerminationStage : IStage
		{
			private Agent agent;

			public TerminationStage(Agent agent)
			{
				// TODO: Complete member initialization
				this.agent = agent;
			}

			void IStage.Enter()
			{
			}

			void IStage.Leave()
			{
			}

			void IStage.Update()
			{
			}
		}
	}
}
