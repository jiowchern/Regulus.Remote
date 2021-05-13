namespace Regulus.Network
{
    
    public interface IWaitableValue<T> 
    {
        IAwaitable<T> GetAwaiter();
        event System.Action<T> ValueEvent;
    }
}