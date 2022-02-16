using System;


namespace Regulus.Remote.Reactive
{
    internal class PropertyObservable<T> : System.Reactive.ObservableBase<T>, IDisposable
    {

        private readonly Property<T> _Property;
        private T _Value;
        IObserver<T> _Observer;
        Regulus.Remote.ThreadUpdater _ThreadUpdater;
        public PropertyObservable(Regulus.Remote.Property<T> property)
        {
            this._Property = property;
            _Value = _Property.Value;
            _ThreadUpdater = new ThreadUpdater(() => { });
            _ThreadUpdater.Start();
        }


        protected override IDisposable SubscribeCore(IObserver<T> observer)
        {

            _Observer = observer;
            _ThreadUpdater.Stop();
            _ThreadUpdater = new ThreadUpdater(() =>
            {
                if (_Value.Equals(_Property.Value))
                {
                    return;
                }
                _Value = _Property.Value;
                _Observer.OnNext(_Value);
            });
            _ThreadUpdater.Start();
            return this;
        }

        void IDisposable.Dispose()
        {
            _ThreadUpdater.Stop();
            if (_Observer != null)
                _Observer.OnCompleted();
        }


    }
}
