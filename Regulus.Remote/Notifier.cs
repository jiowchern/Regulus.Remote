using System;

namespace Regulus.Remote
{
    public struct TypeObject
    {
        public readonly Type Type;
        public readonly object Instance;
        public TypeObject(Type type,object instance)
        {
            Type = type;
            Instance = instance;
        }
    }
    interface ITypeObjectNotifiable
    {
        
        event System.Action<TypeObject> SupplyEvent;
        event System.Action<TypeObject> UnsupplyEvent;
    }
    public interface IObjectAccessible 
    {
        void Add(object instance);
        void Remove(object instance);
    }
    class NotifierUpdater   : System.IDisposable
    {
        private readonly int _Id;
        private readonly ITypeObjectNotifiable _Notifiable;

        public event System.Action<int, TypeObject> SupplyEvent;
        public event System.Action<int, TypeObject> UnsupplyEvent;
        public NotifierUpdater(int id , ITypeObjectNotifiable notifiable)
        {
            this._Id = id;
            this._Notifiable = notifiable;
            _Notifiable.SupplyEvent += _Supply;
            _Notifiable.UnsupplyEvent += _Unsupply;
        }
        
        void IDisposable.Dispose()
        {
            _Notifiable.SupplyEvent -= _Supply;
            _Notifiable.UnsupplyEvent -= _Unsupply;
        }

        private void _Unsupply(TypeObject obj)
        {
            UnsupplyEvent(_Id , obj);
        }

        private void _Supply(TypeObject obj)
        {
            SupplyEvent(_Id, obj);
        }
    }
    public class Notifier<T> : IObjectAccessible , ITypeObjectNotifiable
    {
        readonly NotifierCollection<T> _Collection;
        public readonly INotifier<T> Base;

        public Notifier(NotifierCollection<T> collection)
        {
            _Collection = collection;
            _Collection.Notifier.Supply += _OnSupply;
            _Collection.Notifier.Unsupply += _OnUnsupply;
            Base = _Collection;
            _UnsupplyEvent += _Empty;
            _SupplyEvent += _Empty;
        }

        private void _Empty(TypeObject obj)
        {
            
        }

        public Notifier() : this(new NotifierCollection<T>())
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
            _Collection.Items.Add((T)instance);
        }

        void IObjectAccessible.Remove(object instance)
        {
            _Collection.Items.Remove((T)instance);
        }
    }
}
