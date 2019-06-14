using System.Threading;


using Regulus.Utility;

namespace Regulus.Remote
{
	internal class ValueWaiter<T>
	{
		private readonly Value<T> value;

		private volatile bool _HasValue;

		public T Value { get; private set; }

	    

        public ValueWaiter(Value<T> value)
		{
            _HasValue = false;
            value.OnValue += _Getted;
            this.value = value;
            

        }

		internal void Run(object obj)
		{
			var are = (AutoResetEvent)obj;

            var powerRegulator = new PowerRegulator(10);
            var autoPowerRegulator = new AutoPowerRegulator(powerRegulator);
            
			while(_HasValue == false)
			{
                autoPowerRegulator.Operate();
            }

			are.Set();
		}

		private void _Getted(T obj)
		{
			Value = obj;
			_HasValue = true;
		}
	}
}
