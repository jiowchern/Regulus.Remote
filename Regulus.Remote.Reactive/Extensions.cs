using System;
using System.Reactive.Linq;


namespace Regulus.Remote.Reactive
{
    
    public static class Extensions
    {
        public static IObservable<System.Reactive.Unit> ReturnVoid(this System.Action action)
        {
            return Observable.Defer(() => {
                action();
                return Observable.Return(System.Reactive.Unit.Default);
            });
        }
        public static IObservable<TValue> RemoteValue<TValue>(this Regulus.Remote.Value<TValue> ret)
        {
            return new OnceRemoteReturnValueEvent<TValue>(ret);
        }

        public static IObservable<TValue> PropertyChangeValue<TValue>(this Regulus.Remote.Property<TValue> ret)
        {
            return new PropertyObservable<TValue>(ret);
        }

        public static IObservable<T> SupplyEvent<T>(this Notifier<T> notifier) where T : class
        {
            return SupplyEvent(notifier.Base);
        }
        public static IObservable<T> SupplyEvent<T>(this INotifier<T> notifier)
        {
            return Observable.FromEvent<Action<T>, T>(h => notifier.Supply += h, h => notifier.Supply -= h);            
        }
        public static IObservable<T> UnsupplyEvent<T>(this Notifier<T> notifier) where T: class
        {
            return UnsupplyEvent(notifier.Base);
        }
        public static IObservable<T> UnsupplyEvent<T>(this INotifier<T> notifier) where T : class
        {
            return Observable.FromEvent<Action<T>, T>(h => notifier.Unsupply += h, h => notifier.Unsupply -= h);
        }

        public static IObservable<T> EventObservable<T>(System.Action<System.Action<T>> add_handler, System.Action<System.Action<T>> remove_handler)
        {
            return Observable.FromEvent<Action<T>, T>(add_handler, remove_handler);
        }

        public static IObservable<System.Reactive.Unit> EventObservable(System.Action<System.Action> add_handler, System.Action<System.Action> remove_handler)
        {
            return Observable.FromEvent(add_handler, remove_handler);
        }

    }
}
