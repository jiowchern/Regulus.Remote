using Regulus.Utility;

namespace Regulus.Remote.Ghost
{
	public partial class Agent
	{
		private class TerminationStage : IStatus
		{
			private Agent agent;

			public TerminationStage(Agent agent)
			{
				// TODO: Complete member initialization
				this.agent = agent;
			}

			void IStatus.Enter()
			{
			}

			void IStatus.Leave()
			{
			}

			void IStatus.Update()
			{
			}
		}
	}
}
