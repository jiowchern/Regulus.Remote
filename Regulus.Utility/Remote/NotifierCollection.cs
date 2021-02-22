using System;
using System.Collections;
using System.Collections.Generic;

namespace Regulus.Remote
{
    public class NotifierCollection<T> : INotifier<T>, ICollection<T> , System.Collections.Generic.IReadOnlyCollection<T>
    {
        readonly System.Collections.Generic.List<T> _Items;
        

        int ICollection<T>.Count => _Items.Count;

        bool ICollection<T>.IsReadOnly => Items.IsReadOnly;

        int IReadOnlyCollection<T>.Count => _Items.Count;

        public readonly ICollection<T> Items;
        public readonly IReadOnlyCollection<T> ReadOnlyItems;
        public readonly INotifier<T> Notifier;
        public NotifierCollection() : this(new System.Collections.Generic.List<T>())
        {
        }
        public NotifierCollection(IEnumerable<T> items)
        {
            _Items = new System.Collections.Generic.List<T>(items);
            Items = this;
            ReadOnlyItems = this;
            Notifier = this;

            _UnsupplyEvent += _Empty;
            _SupplyEvent += _Empty;
        }

        private void _Empty(T obj)
        {
        }

        event Action<T> _SupplyEvent;
        event Action<T> INotifier<T>.Supply
        {
            add
            {

                _SupplyEvent += value;
                foreach (var item in _Items)
                {
                    value(item);
                }
            }

            remove
            {
                _SupplyEvent -= value;
            }
        }

        event Action<T> _UnsupplyEvent;
        event Action<T> INotifier<T>.Unsupply
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

        void ICollection<T>.Add(T item)
        {
            _Items.Add(item);
            _SupplyEvent(item);
        }

        void ICollection<T>.Clear()
        {
            foreach (var item in _Items)
            {
                _UnsupplyEvent(item);
            }
            _Items.Clear();

        }

        bool ICollection<T>.Contains(T item)
        {
            return _Items.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            _Items.CopyTo(array, arrayIndex);
        }

        bool ICollection<T>.Remove(T item)
        {

            var result = _Items.Remove(item);
            if (result)
                _UnsupplyEvent(item);
            return result;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Items.GetEnumerator();
        }
    }
}
