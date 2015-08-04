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

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPIs;
using VGame.Project.FishHunter.Play;

#endregion

namespace VGame.Project.FishHunter.Storage
{
	public class Center : ICore
	{
		private readonly Hall _Hall;

		private readonly IStorage _Stroage;

		private readonly Updater _Update;

		public Center(IStorage storage)
		{
			_Stroage = storage;
			_Hall = new Hall();
			_Update = new Updater();
		}

		void ICore.AssignBinder(ISoulBinder binder)
		{
			_Hall.PushUser(new User(binder, _Stroage));
		}

		bool IUpdatable.Update()
		{
			_Update.Working();
			return true;
		}

		void IBootable.Launch()
		{
			_Update.Add(_Hall);
		}

		void IBootable.Shutdown()
		{
			_Update.Shutdown();
		}
	}
}