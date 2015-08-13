
using Regulus.Utility;

using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Storage;

namespace VGame.Project.FishHunter.Formula
{
	internal class BuildStorageControllerStage : IStage
	{
		public delegate void DoneCallback(ExpansionFeature controller);

		public event DoneCallback OnDoneEvent;

		private readonly IUser _User;

		private ExpansionFeature _ExpansionFeature;

		private IAccountFinder _Finder;

		private IFormulaStageDataRecorder _FormulaStageDataRecorder;

		private IFormulaPlayerRecorder _FormulaPlayerRecorder;

		public BuildStorageControllerStage(IUser user)
		{
			_User = user;
		}

		void IStage.Enter()
		{
			_User.QueryProvider<IAccountFinder>().Supply += _GetFinder;
		}

		void IStage.Leave()
		{
		}

		void IStage.Update()
		{
		}

		private void _GetFinder(IAccountFinder obj)
		{
			_User.QueryProvider<IAccountFinder>().Supply -= _GetFinder;

			_Finder = obj;

			_User.QueryProvider<IFormulaStageDataRecorder>().Supply += _GetFishDataLoader;
		}

		private void _GetFishDataLoader(IFormulaStageDataRecorder obj)
		{
			_User.QueryProvider<IFormulaStageDataRecorder>().Supply -= _GetFishDataLoader;

			_FormulaStageDataRecorder = obj;

			_User.QueryProvider<IFormulaPlayerRecorder>().Supply += _GetRecorder;
		}

		private void _GetRecorder(IFormulaPlayerRecorder obj)
		{
			_FormulaPlayerRecorder = obj;
			_User.QueryProvider<IFormulaPlayerRecorder>().Supply -= _GetRecorder;
			_Finish();
		}

		private void _Finish()
		{
			_ExpansionFeature = new ExpansionFeature(_Finder, _FormulaStageDataRecorder, _FormulaPlayerRecorder);

			OnDoneEvent(_ExpansionFeature);
		}
	}
}
