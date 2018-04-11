using System;
using System.Threading;


using Regulus.Collection;

namespace Regulus.Utility
{

    

    public class AsyncExecuter
	{
		private readonly Queue<Action> _Tasks;
	    private readonly System.Threading.Tasks.Task _Task;
	    private volatile bool _Enable;
	    private readonly ManualResetEvent _ResetEvent;
	    public AsyncExecuter()
	    {
	        _ResetEvent = new ManualResetEvent(true);
            _Enable = true;
            _Tasks = new Queue<Action>();
		    _Task = System.Threading.Tasks.Task.Run((Action) _Run);

        }

        private void _Run()
        {
            while (_Enable)
            {
                Action action;
                if (_Tasks.TryDequeue(out action))
                {
                    action();
                }
                else
                {
                    _ResetEvent.Reset();
                    _ResetEvent.WaitOne();
                }
            }

            var actions = _Tasks.DequeueAll();
            foreach (var action in actions)
            {
                action();
            }

        }

        public void Shutdown()
        {
            _Enable = false;
            _ResetEvent.Set();
            _Task.GetAwaiter().GetResult();            
        }

		

		public void Push(Action callback)
		{
            if(_Enable == false)
                throw new Exception("it is shutdown.");
		    _Tasks.Enqueue(callback);
            _ResetEvent.Set();
        }


		

		
	}
}
