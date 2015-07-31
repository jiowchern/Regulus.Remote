// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Flag.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Flag type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Collections;
using System.Collections.Generic;

using ProtoBuf;

#endregion

namespace Regulus.CustomType
{
	[ProtoContract]
	public class Flag<T> : IEnumerable<T>, ICollection<T>
	{
		[ProtoMember(1)]
		private readonly HashSet<T> _Flags;

		private ICollection<T> _Collection
		{
			get { return this._Flags; }
		}

		public bool this[T index]
		{
			get { return this._Get(index); }
			set { this._Set(index, value); }
		}

		public Flag()
		{
			this._Flags = new HashSet<T>();
		}

		public Flag(params T[] args)
		{
			this._Flags = new HashSet<T>(args);
		}

		public Flag(IEnumerable<T> flags)
		{
			this._Flags = new HashSet<T>(flags);
		}

		void ICollection<T>.Add(T item)
		{
			this._Collection.Add(item);
		}

		void ICollection<T>.Clear()
		{
			this._Collection.Clear();
		}

		bool ICollection<T>.Contains(T item)
		{
			return this._Collection.Contains(item);
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			this._Collection.CopyTo(array, arrayIndex);
		}

		int ICollection<T>.Count
		{
			get { return this._Collection.Count; }
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return this._Collection.IsReadOnly; }
		}

		bool ICollection<T>.Remove(T item)
		{
			return this._Collection.Remove(item);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this._Flags.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._Flags.GetEnumerator();
		}

		public static implicit operator Flag<T>(object[] objs)
		{
			var m = new Flag<T>();
			foreach (var o in objs)
			{
				m[(T)o] = true;
			}

			return m;
		}

		private void _Set(T index, bool value)
		{
			if (value)
			{
				if (this._Flags.Contains(index) == false)
				{
					this._Flags.Add(index);
				}
			}
			else
			{
				this._Flags.Remove(index);
			}
		}

		private bool _Get(T index)
		{
			return this._Flags.Contains(index);
		}
	}
}