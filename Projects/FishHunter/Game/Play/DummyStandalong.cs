// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DummyStandalong.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the DummyStandalong type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;

#endregion

namespace VGame.Project.FishHunter.Play
{
	public class DummyStandalong : ICore
	{
		private readonly Center _Center;

		private readonly DummyFrature _Storage;

		private readonly Updater _Updater;

		private ICore _Core
		{
			get { return _Center; }
		}

		public DummyStandalong()
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