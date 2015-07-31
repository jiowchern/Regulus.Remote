// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageMachine.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the EmptyStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Collection;

#endregion

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
			this._StandBys = new Queue<IStage>();
		}

		public void Push(IStage new_stage)
		{
			this._StandBys.Enqueue(new_stage);
		}

		public bool Update()
		{
			this._SetCurrentStage();
			this._UpdateCurrentStage();

			return this.Current != null;
		}

		private void _SetCurrentStage()
		{
			IStage stage;
			if (this._StandBys.TryDequeue(out stage))
			{
				if (this.Current != null)
				{
					this.Current.Leave();
				}

				stage.Enter();
				this.Current = stage;
			}
		}

		private void _UpdateCurrentStage()
		{
			if (this.Current != null)
			{
				this.Current.Update();
			}
		}

		public void Termination()
		{
			this._StandBys.DequeueAll();
			if (this.Current != null)
			{
				this.Current.Leave();
				this.Current = null;
			}
		}

		public void Empty()
		{
			this.Push(new EmptyStage());
		}
	}
}