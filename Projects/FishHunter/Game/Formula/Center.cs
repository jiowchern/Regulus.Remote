// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Center.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the Center type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter.Play;

#endregion

namespace VGame.Project.FishHunter.Formula
{
	public class Center : ICore
	{
		private readonly StorageController _Controller;

		private readonly Hall _Hall;

		private readonly Updater _Updater;

		private Center()
		{
			_Hall = new Hall();
			_Updater = new Updater();
		}

		public Center(StorageController controller) : this()
		{
			// TODO: Complete member initialization
			this._Controller = controller;
		}

		void ICore.AssignBinder(ISoulBinder binder)
		{
			_Hall.PushUser(new User(binder, _Controller));
		}

		bool IUpdatable.Update()
		{
			_Updater.Working();
			return true;
		}

		void IBootable.Launch()
		{
			_Updater.Add(_Hall);
		}

		void IBootable.Shutdown()
		{
			_Updater.Shutdown();
		}
	}
}