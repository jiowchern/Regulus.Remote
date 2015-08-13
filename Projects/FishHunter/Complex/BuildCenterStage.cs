
using Regulus.Utility;

using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Formula;

namespace VGame.Project.FishHunter
{
	internal class BuildCenterStage : IStage
	{
		public delegate void SuccessBuiledCallback(ExternalFeature features);

		public event SuccessBuiledCallback OnBuiledEvent;

		public struct ExternalFeature
		{
			public IAccountFinder AccountFinder;

			public IFishStageQueryer FishStageQueryer;

			public IGameRecorder GameRecorder;

			public ITradeNotes TradeAccount;

			public IFormulaPlayerRecorder FormulaPlayerRecorder;
		}

		private readonly IUser _FormulaUser;

		private readonly Storage.IUser _StorageUser;

		private ExternalFeature _Feature;

		public BuildCenterStage(IUser formula_user, Storage.IUser storage_user)
		{
			_Feature = new ExternalFeature();
			_FormulaUser = formula_user;
			_StorageUser = storage_user;
		}

		void IStage.Enter()
		{
			_FormulaUser.FishStageQueryerProvider.Supply += _GetFishStageQuery;
		}

		void IStage.Leave()
		{
		}

		void IStage.Update()
		{
		}

		private void _GetFishStageQuery(IFishStageQueryer obj)
		{
			_FormulaUser.FishStageQueryerProvider.Supply -= _GetFishStageQuery;
			_Feature.FishStageQueryer = obj;

			_StorageUser.QueryProvider<IAccountFinder>().Supply += _AccountFinder;
		}

		private void _AccountFinder(IAccountFinder obj)
		{
			_StorageUser.QueryProvider<IAccountFinder>().Supply -= _AccountFinder;
			_Feature.AccountFinder = obj;

			_StorageUser.QueryProvider<ITradeNotes>().Supply += BuildCenterStage_Supply;
		}

		private void BuildCenterStage_Supply(ITradeNotes obj)
		{
			_StorageUser.QueryProvider<ITradeNotes>().Supply -= BuildCenterStage_Supply;
			_Feature.TradeAccount = obj;

			_StorageUser.QueryProvider<IGameRecorder>().Supply += _RecordQueriers;
		}

		private void _RecordQueriers(IGameRecorder obj)
		{
			_StorageUser.QueryProvider<IGameRecorder>().Supply -= _RecordQueriers;
			_Feature.GameRecorder = obj;

			_StorageUser.QueryProvider<IFormulaPlayerRecorder>().Supply += FormulaPlayerRecord;
		}

		private void FormulaPlayerRecord(IFormulaPlayerRecorder obj)
		{
			_Feature.FormulaPlayerRecorder = obj;

			_StorageUser.QueryProvider<IFormulaPlayerRecorder>().Supply -= FormulaPlayerRecord;

			OnBuiledEvent(_Feature);
		}
	}
}
