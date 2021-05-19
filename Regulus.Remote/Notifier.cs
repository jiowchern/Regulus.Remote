using System;
using System.Linq;

namespace Regulus.Remote
{
    public class Notifier<T> : IObjectAccessible , ITypeObjectNotifiable ,System.IDisposable      where T : class   
    {
        
        public readonly INotifier<T> Base;
        readonly System.Collections.Generic.ICollection<T> _Collection;
        readonly Regulus.Remote.NotifiableCollection<TypeObject> _TypeObjects;

        public Notifier(INotifier<T> notifier , System.Collections.Generic.ICollection<T>  collection)
        {
            _TypeObjects = new NotifiableCollection<TypeObject>();
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
            
            
            lock(_TypeObjects)
            {
                var items = from item in _TypeObjects.Items where item.Instance == obj && item.Type == typeof(T) select item;
                var i = items.First();
                _TypeObjects.Items.Remove(i);
            }
            
        }

        private void _OnSupply(T obj)
        {
            
            var to = new TypeObject(typeof(T), obj);
            lock(_TypeObjects)
                _TypeObjects.Items.Add(to);            
        }

        
        event Action<TypeObject> ITypeObjectNotifiable.SupplyEvent
        {
            add
            {

                _TypeObjects.Notifier.Supply += value;
            }

            remove
            {
                _TypeObjects.Notifier.Supply -= value;
            }
        }

        public void Dispose()
        {
            Base.Supply -= _OnSupply;
            Base.Unsupply -= _OnUnsupply;
        }

        
        event Action<TypeObject> ITypeObjectNotifiable.UnsupplyEvent
        {
            add
            {
                _TypeObjects.Notifier.Unsupply += value;
            }

            remove
            {
                _TypeObjects.Notifier.Unsupply -= value;
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
        

        void IDisposable.Dispose()
        {
            Dispose();
        }
    }
}
