namespace Regulus.Utility
{
    public interface IStatus
    {
        void Enter();

        void Leave();

        void Update();
    }



    public interface IStatus<T>
    {
        void Enter();

        void Leave();

        void Update(T obj);
    }
}
