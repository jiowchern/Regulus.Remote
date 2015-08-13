using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Net45
{
	internal class ValueSpin<T>
	{
		private readonly Value<T> value;

		private volatile bool _HasValue;

		public T Value { get; private set; }

		public ValueSpin(Value<T> value)
		{
			this.value = value;
			_HasValue = false;
		}

		internal T Wait()
		{
			value.OnValue += _Getted;

			var sw = new SpinWait();
			while(_HasValue == false)
			{
				sw.SpinOnce();
			}

			return Value;
		}

		internal void Run(object obj)
		{
			value.OnValue += _Getted;

			var sw = new SpinWait();
			while(_HasValue == false)
			{
				sw.SpinOnce();
			}
		}

		private void _Getted(T obj)
		{
			Value = obj;
			_HasValue = true;
		}
	}
}
