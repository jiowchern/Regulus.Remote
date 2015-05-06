using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class AsyncExecuter
    {
        Regulus.Collection.Queue<Action> _Tasks;
        volatile int _Count;
        public AsyncExecuter(params Action[] callbacks)
        {
            _Tasks = new Collection.Queue<Action>();

            foreach(var task in callbacks)
            {
                Push(task);
            }
        }

        public void WaitDone()
        {
            while (IsDone() == false) ; 
        }

        internal bool IsDone()
        {
            lock(this)
                return _Count == 0;
        }

        public void Push(Action callback)
        {
            bool execute = false;
            lock(this)
            {
                execute = _Count == 0;
                _Count++;
                _Tasks.Enqueue(callback);
            }

            if (execute)
            {
                _Execute();
            }            
        }

        private void _Execute()
        {            
            Action task;
            if (_Tasks.TryDequeue(out task))
            {                
                System.Threading.ThreadPool.QueueUserWorkItem(_Run , task);                    
            }            
        }

        private void _Run(object state)
        {

            Action task = (Action)state;
            task();
            lock (this)
                _Count--;
            _Execute();
        }
    }
}
