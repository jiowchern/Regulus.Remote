using System.Threading;


using Regulus.Utility;

namespace Regulus.Remoting.Ghost.Native
{
	/*internal class IOHandler : Singleton<IOHandler>
	{
		private readonly PowerRegulator _PowerRegulator;

		private readonly Updater _Updater;

		private FPSCounter _Fps;

		private volatile bool _ThreadEnable;

		public int Fps
		{
			get { return _PowerRegulator.FPS; }
		}

		public float Power
		{
			get { return _PowerRegulator.Power; }
		}

		public IOHandler()
		{
			_PowerRegulator = new PowerRegulator(30);
			_Updater = new Updater();
		}

		public void Stop(IUpdatable updater)
		{
			_Updater.Remove(updater);
		}

		public void Start(IUpdatable updater)
		{
			_Updater.Add(updater);
			_Launch();
		}

		private void _Launch()
		{
			if(_ThreadEnable == false)
			{
				_ThreadEnable = true;
				ThreadPool.QueueUserWorkItem(_Handle);
			}
		}

		private void _Handle(object obj)
		{
			var sw = new SpinWait();
			long response = 0;
			do
			{
				var current = Agent.ResponsePackages + Agent.RequestPackages;
				_PowerRegulator.Operate(current);

				response = current;
				_Updater.Working();
			}
			while(_Updater.Count > 0);

			_Shutdown();
		}

		private void _Shutdown()
		{
			_ThreadEnable = false;
		}
	}*/
}
