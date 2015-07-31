// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Stage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Regulus.Utility
{
	public interface IStage
	{
		void Enter();

		void Leave();

		void Update();
	}


	public class StageLock
	{
		public enum Status
		{
			Locked, 

			Unlock
		}

		public Status Current { get; private set; }

		public StageLock()
		{
			this.Current = Status.Locked;
		}

		public void Unlock()
		{
			this.Current = Status.Unlock;
		}
	}

	public interface IStage<T>
	{
		StageLock Enter(T obj);

		void Leave(T obj);

		void Update(T obj);
	}
}