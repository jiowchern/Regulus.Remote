namespace Regulus.Lockstep
{
    public interface ICommandProvidable<TCommand>
    {
        TCommand Current { get; }
    }
}