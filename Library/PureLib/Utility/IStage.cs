namespace Regulus.Utility
{
	public interface IStage
	{
		void Enter();

		void Leave();

		void Update();
	}

	

	public interface IStage<T> 
    {
        void Enter();

        void Leave();

        void Update(T obj);
	}
}
