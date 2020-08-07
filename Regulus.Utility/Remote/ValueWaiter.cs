using Regulus.Utility;
using System.Threading;

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
            AutoResetEvent are = (AutoResetEvent)obj;

            PowerRegulator powerRegulator = new PowerRegulator(10);
            AutoPowerRegulator autoPowerRegulator = new AutoPowerRegulator(powerRegulator);

            while (_HasValue == false)
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
