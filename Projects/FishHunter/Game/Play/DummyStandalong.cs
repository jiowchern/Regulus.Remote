<<<<<<< HEAD
﻿using Regulus.Framework;
=======
﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DummyStandalone.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the DummyStandalone type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Regulus.Framework;
>>>>>>> bb08c0b8a8aa5ec0c708cd9f624c302cd192eb5d
using Regulus.Remoting;
using Regulus.Utility;

namespace VGame.Project.FishHunter.Play
{
	public class DummyStandalone : ICore
	{
		private readonly Center _Center;

		private readonly DummyFrature _Storage;

		private readonly Updater _Updater;

		private ICore _Core
		{
			get { return _Center; }
		}

		public DummyStandalone()
		{
			_Storage = new DummyFrature();
			_Updater = new Updater();
			_Center = new Center(_Storage, _Storage, _Storage, _Storage);
		}

		void ICore.AssignBinder(ISoulBinder binder)
		{
			_Core.AssignBinder(binder);
		}

		bool IUpdatable.Update()
		{
			_Updater.Working();
			return true;
		}

		void IBootable.Launch()
		{
			_Updater.Add(_Center);
		}

		void IBootable.Shutdown()
		{
			_Updater.Shutdown();
		}
	}
}
