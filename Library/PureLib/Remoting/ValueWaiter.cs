using System.Threading;


using Regulus.Utility;

namespace Regulus.Remoting
{
	internal class ValueWaiter<T>
	{
		private readonly Value<T> value;

		private volatile bool _HasValue;

		public T Value { get; private set; }

	    

        public ValueWaiter(Value<T> value)
		{
            value.OnValue += _Getted;
            this.value = value;
			_HasValue = false;

            

        }

		internal void Run(object obj)
		{
			var are = (AutoResetEvent)obj;

            var powerRegulator = new PowerRegulator(10);
		    FPSCounter counter = new FPSCounter();
			while(_HasValue == false)
			{                
                powerRegulator.Operate(counter.Value);
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
