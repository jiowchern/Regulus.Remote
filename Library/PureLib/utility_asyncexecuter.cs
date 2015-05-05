using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class AsyncExecuter
    {
        Action<object> _Async;
        Regulus.Collection.Queue<Action> _Actions;        
        private IAsyncResult _AsyncResult;

        public AsyncExecuter(params Action[] callbacks)
        {
            _InitialEmptyCall();            
            _Actions = new Regulus.Collection.Queue<Action>(callbacks);            
            _Run();
        }

        private void _InitialEmptyCall()
        {
            _Actions = new Collection.Queue<Action>();
            _Async = _Run;
            _AsyncResult = _Async.BeginInvoke(null, null, null);
            _Async.EndInvoke(_AsyncResult);
        }

        private void _Run()
        {
            if (_AsyncResult.IsCompleted)
                _AsyncResult =  _Async.BeginInvoke(null, null, null);
        }

        

        private void _Run(object state)
        {
            Action[] callbacks = _Actions.DequeueAll();

            foreach (var c in callbacks)
                c();
        }

        public void WaitDone()
        {
            while (! _AsyncResult.IsCompleted ) ;
        }

        internal bool IsDone()
        {
            return _AsyncResult.IsCompleted;
        }

        public void Push(Action callback)
        {
            _Actions.Enqueue(callback);
            _Run();
        }
    }
}
