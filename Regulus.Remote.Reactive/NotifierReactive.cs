using System;
using System.Reactive.Linq ;


namespace Regulus.Remote.Reactive
{
    
    internal class OnceRemoteReturnValueEvent<T> : System.Reactive.ObservableBase<T>, IDisposable
    {
        private Value<T> _Return;
        IObserver<T> _Observer;
        public OnceRemoteReturnValueEvent(Value<T> ret) 
        {
            this._Return = ret;
        }

        private void _OnValue(T obj)
        {
            _Observer.OnNext(obj);
            _Observer.OnCompleted();
        }

        
        void IDisposable.Dispose()
        {
            _Return.OnValue -= _OnValue;

        }

        protected override IDisposable SubscribeCore(IObserver<T> observer)
        {            
            _Observer = observer;
            _Return.OnValue += _OnValue;
            return this;
        }
    }

    public static class NotifierReactive
    {
        public static IObservable<TValue> RemoteValue<TValue>(this Regulus.Remote.Value<TValue> ret)
        {
            return new OnceRemoteReturnValueEvent<TValue>(ret);
        }

        public static IObservable<T> SupplyEvent<T>(this Notifier<T> notifier)
        {
            return SupplyEvent(notifier.Base);
        }
        public static IObservable<T> SupplyEvent<T>(this INotifier<T> notifier)
        {
            return Observable.FromEvent<Action<T>, T>(h => notifier.Supply += h, h => notifier.Supply -= h);            
        }
        public static IObservable<T> UnsupplyEvent<T>(this Notifier<T> notifier)
        {
            return UnsupplyEvent(notifier.Base);
        }
        public static IObservable<T> UnsupplyEvent<T>(this INotifier<T> notifier)
        {
            return Observable.FromEvent<Action<T>, T>(h => notifier.Unsupply += h, h => notifier.Unsupply -= h);
        }
    }
}
