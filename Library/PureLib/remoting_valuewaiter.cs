using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{
    class ValueWaiter<T>
    {
        private Value<T> value;
        volatile bool _HasValue;
        T _Value;

        public T Value { get { return _Value; } }
        public ValueWaiter(Value<T> value)
        {
            // TODO: Complete member initialization
            this.value = value;
            _HasValue = false;

        }

        internal void Run(object obj)
        {
            System.Threading.AutoResetEvent are = (System.Threading.AutoResetEvent)obj;
            value.OnValue += _Getted;


            int count = 0;
            while (_HasValue == false)
            {
                count++;
                if (count % 10 == 0)
                {
                    System.Threading.Thread.Sleep(0);
                }
                else if (count % 20 == 0)
                {
                    System.Threading.Thread.Sleep(1);
                }
                else 
                    System.Threading.Thread.SpinWait(count);

                if (count > 20 * 1000)
                {
                    count = 0;
                    System.Threading.Thread.Sleep(1000);
                }
            }


            are.Set();
            
        }

        private void _Getted(T obj)
        {
            _Value = obj;
            _HasValue = true;            
        }
    }
}
