using Regulus.Utility;
using Regulus.Utility;
using System;

namespace Regulus.Remote
{
	public class ThreadUpdater 
	{
		private readonly System.Action _Updater;

		private volatile bool _Enable;
		readonly System.Threading.Tasks.Task _Task;
		public ThreadUpdater(System.Action updater)
		{
			_Updater = updater;
			
			_Task = new System.Threading.Tasks.Task(_Update);
		}

		void _Update()
		{
			var regulator = new AutoPowerRegulator(new PowerRegulator());			
			
			while (_Enable)
            {
				_Updater();
				regulator.Operate();
			}
			
		}

		
		

        public void Start()
        {
			_Enable = true;
			_Task.Start();

		}

        public void Stop()
        {
			_Enable = false;
			_Task.Wait();

		}
    }
}
