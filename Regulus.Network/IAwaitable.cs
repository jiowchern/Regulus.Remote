using System.Runtime.CompilerServices;

namespace Regulus.Network
{
    public interface IAwaitable<T> : INotifyCompletion
    {
        
        bool IsCompleted { get; }

        T GetResult();        
    }
}