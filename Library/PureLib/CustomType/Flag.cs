using System.Collections;
using System.Collections.Generic;


using ProtoBuf;

namespace Regulus.CustomType
{
	[ProtoContract]
	public class Flag<T> : IEnumerable<T>, ICollection<T>
	{
		[ProtoMember(1)]
		private readonly HashSet<T> _Flags;

		private ICollection<T> _Collection
		{
			get { return _Flags; }
		}

		public bool this[T index]
		{
			get { return _Get(index); }
			set { _Set(index, value); }
		}

		public Flag()
		{
			_Flags = new HashSet<T>();
		}

		public Flag(params T[] args)
		{
			_Flags = new HashSet<T>(args);
		}

		public Flag(IEnumerable<T> flags)
		{
			_Flags = new HashSet<T>(flags);
		}

		void ICollection<T>.Add(T item)
		{
			_Collection.Add(item);
		}

		void ICollection<T>.Clear()
		{
			_Collection.Clear();
		}

		bool ICollection<T>.Contains(T item)
		{
			return _Collection.Contains(item);
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			_Collection.CopyTo(array, arrayIndex);
		}

		int ICollection<T>.Count
		{
			get { return _Collection.Count; }
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return _Collection.IsReadOnly; }
		}

		bool ICollection<T>.Remove(T item)
		{
			return _Collection.Remove(item);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return _Flags.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _Flags.GetEnumerator();
		}

		public static implicit operator Flag<T>(object[] objs)
		{
			var m = new Flag<T>();
			foreach(var o in objs)
			{
				m[(T)o] = true;
			}

			return m;
		}

		private void _Set(T index, bool value)
		{
			if(value)
			{
				if(_Flags.Contains(index) == false)
				{
					_Flags.Add(index);
				}
			}
			else
			{
				_Flags.Remove(index);
			}
		}

		private bool _Get(T index)
		{
			return _Flags.Contains(index);
		}
	}
}
