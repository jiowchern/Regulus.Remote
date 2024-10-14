using System;

namespace Regulus.Remote
{

    public class Property<T> : IDirtyable, IAccessable //, IEquatable<Property<T>>
    {
        T _Value;
        public Property(T val) : this()
        {
            _Value = val;

        }
        public Property()
        {
            DirtyEvent += (o) => { };
        }
        public T Value
        {
            get
            {                
                return _Value;
            }
            set
            {
                _SetValue(value);

            }
        }

        private void _SetValue(T value)
        {            
            _Value = value;            
            DirtyEvent(_Value);
        }

        public event Action<object> DirtyEvent;
        event Action<object> IDirtyable.ChangeEvent
        {
            add
            {
                DirtyEvent += value;
            }

            remove
            {
                DirtyEvent -= value;
            }
        }

        object IAccessable.Get()
        {
            return _Value;
        }

        void IAccessable.Set(object value)
        {
            _Value = (T)value;
        }

        /* bool IEquatable<Property<T>>.Equals(Property<T> other)
         {
             return _EqualElement(other);
         }*/

        private bool _EqualElement(Property<T> other)
        {
            IEquatable<T> equipable = other.Value as IEquatable<T>;
            if (equipable != null)
                return equipable.Equals(_Value);
            return false;
        }

        public static bool operator ==(Property<T> obj1, Property<T> obj2)
        {
            return obj1._EqualElement(obj2);
        }

        public static bool operator !=(Property<T> obj1, Property<T> obj2)
        {
            return !obj1._EqualElement(obj2);
        }

        public static implicit operator T(Property<T> p)
        {
            return p.Value;
        }
    }
}
