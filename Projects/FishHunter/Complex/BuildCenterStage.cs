
using Regulus.Utility;

using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Formula;

namespace VGame.Project.FishHunter
{
	internal class BuildCenterStage : IStage
	{
		public event SuccessBuiledCallback OnBuiledEvent;

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

		public struct ExternalFeature
		{
			public IAccountFinder AccountFinder;

			public IFishStageQueryer FishStageQueryer;

			public IRecordHandler RecordHandler;

			public ITradeNotes TradeAccount;
		}

		public delegate void SuccessBuiledCallback(ExternalFeature features);

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

			_StorageUser.QueryProvider<IRecordHandler>().Supply += _RecordQueriers;
		}

		private void _RecordQueriers(IRecordHandler obj)
		{
			_StorageUser.QueryProvider<IRecordHandler>().Supply -= _RecordQueriers;
			_Feature.RecordHandler = obj;

			OnBuiledEvent(_Feature);
		}
	}
}