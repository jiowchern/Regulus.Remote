using System;

namespace Regulus.Remote
{
    public class Notifier<T> : IObjectAccessible , ITypeObjectNotifiable ,System.IDisposable        
    {
        
        public readonly INotifier<T> Base;
        readonly System.Collections.Generic.ICollection<T> _Collection;

        public Notifier(INotifier<T> notifier , System.Collections.Generic.ICollection<T>  collection)
        {
            _UnsupplyEvent += _Empty;
            _SupplyEvent += _Empty;

            _Collection = collection;
            Base = notifier;
            Base.Supply += _OnSupply;
            Base.Unsupply += _OnUnsupply;                     
        }
        public Notifier(NotifiableCollection<T> collection) : this(collection, collection)
        {
            
        }

        public Notifier() : this(new NotifiableCollection<T>())
        {            
        }

        private void _OnUnsupply(T obj)
        {
            _UnsupplyEvent(new TypeObject(typeof(T), obj));
        }

        private void _OnSupply(T obj)
        {
            _SupplyEvent(new TypeObject(typeof(T) , obj));
        }

        event Action<TypeObject> _SupplyEvent;
        event Action<TypeObject> ITypeObjectNotifiable.SupplyEvent
        {
            add
            {
                _SupplyEvent += value;
            }

            remove
            {
                _SupplyEvent -= value;
            }
        }

        public void Dispose()
        {
            Base.Supply -= _OnSupply;
            Base.Unsupply -= _OnUnsupply;
        }

        event Action<TypeObject> _UnsupplyEvent;
        event Action<TypeObject> ITypeObjectNotifiable.UnsupplyEvent
        {
            add
            {
                _UnsupplyEvent += value;
            }

            remove
            {
                _UnsupplyEvent -= value;
            }
        }

        void IObjectAccessible.Add(object instance)
        {
            _Collection.Add((T)instance);
        }

        void IObjectAccessible.Remove(object instance)
        {
            _Collection.Remove((T)instance);
        }
        private void _Empty(TypeObject obj)
        {

        }

        void IDisposable.Dispose()
        {
            Dispose();
        }
    }
}
