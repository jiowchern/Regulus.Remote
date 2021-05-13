using System;
using System.Runtime.CompilerServices;

namespace Regulus.Network
{
    public class NoWaitValue<T> : IWaitableValue<T> , IAwaitable<T>
    {
        private readonly Regulus.Remote.Value<T> _Value;

        public NoWaitValue(T val)
        {
            _Value = new Remote.Value<T>(val);
        }

        bool IAwaitable<T>.IsCompleted => _Value.HasValue();

        event Action<T> IWaitableValue<T>.ValueEvent
        {
            add
            {
                _Value.OnValue += value;
            }

            remove
            {
                _Value.OnValue -= value;
            }
        }

        IAwaitable<T> IWaitableValue<T>.GetAwaiter()
        {
            return this;
        }

        T IAwaitable<T>.GetResult()
        {
            return _Value.GetValue();
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            continuation?.Invoke();
        }
    }
}