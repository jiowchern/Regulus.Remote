using System;

namespace Regulus.Remote
{
    public class Property<T> : IDirtyable , IAccessable
	{
		T _Value;
		public Property()
		{
			DirtyEvent += (t,o) => { };
		}
		public T Value { get {
				return _Value;
			} 
			set {
				_Value = value;
				DirtyEvent(typeof(T),_Value);
			} }

		public event Action<Type, object> DirtyEvent;
		event Action<Type, object> IDirtyable.DirtyEvent
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

		public static implicit operator Property<T>(T point)
		{
			var p = new Property<T>();
			p.Value = point;
			return p;
		}

		public static implicit operator T(Property<T> p)
		{			
			return p.Value;
		}
	}
}
