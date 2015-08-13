using Regulus.Collection;

namespace Regulus.Utility
{
	public class EmptyStage : IStage
	{
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

	public class StageMachine
	{
		private readonly Queue<IStage> _StandBys;

		public IStage Current { get; private set; }

		public StageMachine()
		{
			_StandBys = new Queue<IStage>();
		}

		public void Push(IStage new_stage)
		{
			_StandBys.Enqueue(new_stage);
		}

		public bool Update()
		{
			_SetCurrentStage();
			_UpdateCurrentStage();

			return Current != null;
		}

		private void _SetCurrentStage()
		{
			IStage stage;
			if(_StandBys.TryDequeue(out stage))
			{
				if(Current != null)
				{
					Current.Leave();
				}

				stage.Enter();
				Current = stage;
			}
		}

		private void _UpdateCurrentStage()
		{
			if(Current != null)
			{
				Current.Update();
			}
		}

		public void Termination()
		{
			_StandBys.DequeueAll();
			if(Current != null)
			{
				Current.Leave();
				Current = null;
			}
		}

		public void Empty()
		{
			Push(new EmptyStage());
		}
	}
}
