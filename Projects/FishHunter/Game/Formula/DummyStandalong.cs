// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DummyStandalone.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the DummyStandalone type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;


using VGame.Project.FishHunter.Play;

namespace VGame.Project.FishHunter.Formula
{
	public class DummyStandalone : ICore
	{
		private readonly Center _Center;

		private readonly ExpansionFeature _Storage;

		private readonly Updater _Updater;

		private ICore _Core
		{
			get { return _Center; }
		}

		public DummyStandalone()
		{
			var frature = new DummyFrature();

			_Storage = new ExpansionFeature(frature, frature, frature);
			_Updater = new Updater();
			_Center = new Center(_Storage);
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
