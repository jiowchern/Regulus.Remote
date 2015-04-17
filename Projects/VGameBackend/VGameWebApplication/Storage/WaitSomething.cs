using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGameWebApplication.Storage
{
    public abstract class WaitSomething<T>
    {
        private bool _Enable;

        protected abstract void Start();
        protected abstract T End();


        public System.Threading.Tasks.Task<T> Handle()
        {
            _Enable = true;
            var task = new System.Threading.Tasks.Task<T>(_Handle);
            task.Start();
            return task;
        }

        private T _Handle()
        {
            System.Threading.SpinWait sw = new System.Threading.SpinWait();
            Start();
            while (_Enable)
            {
                sw.SpinOnce();
            }
            return End();
        }
        public void Stop()
        {
            _Enable = false;
        }
    }
}
