using System;
using System.Threading;


using Regulus.Collection;

namespace Regulus.Utility
{

    

    public class AsyncExecuter
	{
		private readonly System.Collections.Concurrent.ConcurrentQueue<Action> _Tasks;
	    private readonly System.Threading.Tasks.Task _Task;
	    private volatile bool _Enable;
	    private readonly ManualResetEvent _ResetEvent;
	    public AsyncExecuter()
	    {
	        _ResetEvent = new ManualResetEvent(true);
            _Enable = true;
            _Tasks = new System.Collections.Concurrent.ConcurrentQueue<Action>();
		    _Task = System.Threading.Tasks.Task.Run((Action) _Run);

        }

        private void _Run()
        {
            var regulator =  new Regulus.Utility.AutoPowerRegulator(new PowerRegulator());
            while (_Enable)
            {
                regulator.Operate();
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


            _ExecuteAll();
        }

	    private void _ExecuteAll()
	    {
	        Action action;
	        while (_Tasks.TryDequeue(out action))
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
		    _Tasks.Enqueue(callback);
            _ResetEvent.Set();
        }


		

		
	}
}
