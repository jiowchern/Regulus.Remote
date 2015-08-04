// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildStorageControllerStage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the BuildStorageControllerStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Storage;

#endregion

namespace VGame.Project.FishHunter.Formula
{
	internal class BuildStorageControllerStage : IStage
	{
		public event DoneCallback DoneEvent;

		private readonly IUser _User;

		private StorageController _Controller;

		private IAccountFinder _Finder;

		public BuildStorageControllerStage(IUser user)
		{
			this._User = user;
		}

		void IStage.Enter()
		{
			this._User.QueryProvider<IAccountFinder>().Supply += this._GetFinder;
		}

		void IStage.Leave()
		{
			this._User.QueryProvider<IAccountFinder>().Supply -= this._GetFinder;
		}

		void IStage.Update()
		{
		}

		public delegate void DoneCallback(StorageController controller);

		private void _GetFinder(IAccountFinder obj)
		{
			this._Finder = obj;

			this._Controller = new StorageController(this._Finder);
			this.DoneEvent(this._Controller);
		}
	}
}