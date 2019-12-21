using System.Threading;


using Regulus.Framework;
using Regulus.Utility;

namespace Regulus.Remote
{
	public class PackageRecorder : IUpdatable
	{
		public delegate void ChangeCallback();

		public event ChangeCallback ChangeEvent;

		private readonly TimeCounter _Counter;

		public long _SecondBytes;

		public long TotalBytes { get; private set; }

		public long SecondBytes { get; private set; }

		public PackageRecorder()
		{
			_Counter = new TimeCounter();
		}

		bool IUpdatable.Update()
		{
			lock(_Counter)
			{
				if(_Counter.Second > 1)
				{
					SecondBytes = _SecondBytes;
					_SecondBytes = 0;
					_Counter.Reset();
				}
			}

			return true;
		}

		void IBootable.Launch()
		{
			lock(_Counter)
				_Counter.Reset();
		}

		void IBootable.Shutdown()
		{
			lock(_Counter)
				SecondBytes = 0;
		}

		internal void Set(int size)
		{
			lock(_Counter)
			{
				TotalBytes += size;
				_SecondBytes += size;
				ChangeEvent();
			}
		}
	}

	public class NetworkMonitor : Singleton<NetworkMonitor>
	{
		private volatile bool _Reset;

		private volatile bool _ThreadEnable;

		public PackageRecorder Read { get; private set; }

		public PackageRecorder Write { get; private set; }

		public NetworkMonitor()
		{
			Read = new PackageRecorder();
			Read.ChangeEvent += _ResetTime;
			Write = new PackageRecorder();
			Write.ChangeEvent += _ResetTime;
		}

		private void _ResetTime()
		{
			if(_ThreadEnable == false)
			{
				_ThreadEnable = true;
				ThreadPool.QueueUserWorkItem(_Update);
			}

			_Reset = true;
		}

		private void _Update(object state)
		{
			var updater = new Updater();
			updater.Add(Read);
			updater.Add(Write);

			var counter = new TimeCounter();
			do
			{
				updater.Working();

				if(_Reset)
				{
					counter.Reset();
					_Reset = false;
				}

				Thread.Sleep(1000);
			}
			while(counter.Second <= 30);
			updater.Shutdown();
			_ThreadEnable = false;
		}
	}
}
