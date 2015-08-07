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
		public delegate void DoneCallback(ExpansionFeature controller);

		public event DoneCallback OnDoneEvent;

		private readonly IUser _User;

		private ExpansionFeature _ExpansionFeature;

		private IAccountFinder _Finder;

		private IFishStageDataHandler _StageDataHandler;

		public BuildStorageControllerStage(IUser user)
		{
			_User = user;
		}

		void IStage.Enter()
		{
			_User.QueryProvider<IAccountFinder>().Supply += _GetFinder;
		}

		private void _GetFinder(IAccountFinder obj)
		{
			_User.QueryProvider<IAccountFinder>().Supply -= _GetFinder;
			
			_Finder = obj;
			
			_User.QueryProvider<IFishStageDataHandler>().Supply += _GetFishDataLoader;
		}

		private void _GetFishDataLoader(IFishStageDataHandler obj)
		{
			_User.QueryProvider<IFishStageDataHandler>().Supply -= _GetFishDataLoader;
			
			_StageDataHandler = obj;
			
			_Finish();
		}

		void IStage.Leave()
		{
		}

		void IStage.Update()
		{
		}

		private void _Finish()
		{
			_ExpansionFeature = new ExpansionFeature(_Finder, _StageDataHandler);

			OnDoneEvent(_ExpansionFeature);
		}
	}
}