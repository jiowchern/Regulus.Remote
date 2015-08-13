using System.Collections.Generic;


using Regulus.Utility;

namespace Regulus.Remoting
{
	public interface IGPIBinderFactory
	{
		GPIBinder<T> Create<T>(INotifier<T> notice) where T : class;
	}

	public class GPIBinderFactory : IGPIBinderFactory
	{
		private struct Data
		{
			public IGPIBinder Binder;
		}

		private readonly List<Data> _Binders;

		private readonly Command _Command;

		public GPIBinderFactory(Command command)
		{
			_Command = command;
			_Binders = new List<Data>();
		}

		GPIBinder<T> IGPIBinderFactory.Create<T>(INotifier<T> notice)
		{
			var binder = new GPIBinder<T>(notice, _Command);
			_Binders.Add(
				new Data
				{
					Binder = binder
				});
			return binder;
		}

		public void Setup()
		{
			foreach(var binder in  _Binders)
			{
				binder.Binder.Launch();
			}
		}

		public void Remove()
		{
			foreach(var binder in  _Binders)
			{
				binder.Binder.Shutdown();
			}

			_Binders.Clear();
		}
	}
}
