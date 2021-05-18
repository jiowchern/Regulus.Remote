using System;


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
}
