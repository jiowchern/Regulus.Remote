// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GPIBinderFactory.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IGPIBinderFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Collections.Generic;

using Regulus.Utility;

#endregion

namespace Regulus.Remoting
{
	public interface IGPIBinderFactory
	{
		GPIBinder<T> Create<T>(INotifier<T> notice) where T : class;
	}

	public class GPIBinderFactory : IGPIBinderFactory
	{
		private readonly List<Data> _Binders;

		private readonly Command _Command;

		public GPIBinderFactory(Command command)
		{
			this._Command = command;
			this._Binders = new List<Data>();
		}

		public GPIBinder<T> Create<T>(INotifier<T> notice) where T : class
		{
			var binder = new GPIBinder<T>(notice, this._Command);
			this._Binders.Add(new Data
			{
				Binder = binder
			});
			return binder;
		}

		private struct Data
		{
			public IGPIBinder Binder;
		}

		public void Setup()
		{
			foreach (var binder in  this._Binders)
			{
				binder.Binder.Launch();
			}
		}

		public void Remove()
		{
			foreach (var binder in  this._Binders)
			{
				binder.Binder.Shutdown();
			}

			this._Binders.Clear();
		}
	}
}