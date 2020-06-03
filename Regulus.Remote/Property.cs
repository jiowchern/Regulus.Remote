using System;

namespace Regulus.Remote
{
    public class Property<T> : IDirtyable , IAccessable
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
		public T Value { get {
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
		event Action< object> IDirtyable.DirtyEvent
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
		

		public static implicit operator T(Property<T> p)
		{			
			return p.Value;
		}
	}
}
