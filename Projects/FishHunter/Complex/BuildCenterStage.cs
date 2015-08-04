// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildCenterStage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the BuildCenterStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Formula;

#endregion

namespace VGame.Project.FishHunter
{
	internal class BuildCenterStage : IStage
	{
		public event SuccessBuiledCallback BuiledEvent;

		private readonly IUser _FormulaUser;

		private readonly Storage.IUser _StorageUser;

		private ExternalFeature _Feature;

		public BuildCenterStage(IUser _FormulaUser, Storage.IUser _StorageUser)
		{
			this._Feature = new ExternalFeature();
			this._FormulaUser = _FormulaUser;
			this._StorageUser = _StorageUser;
		}

		void IStage.Enter()
		{
			this._FormulaUser.FishStageQueryerProvider.Supply += this._GetFishStageQuery;
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

			public IRecordQueriers RecordQueriers;

			public ITradeNotes TradeAccount;
		}

		public delegate void SuccessBuiledCallback(ExternalFeature features);

		private void _GetFishStageQuery(IFishStageQueryer obj)
		{
			this._FormulaUser.FishStageQueryerProvider.Supply -= this._GetFishStageQuery;
			this._Feature.FishStageQueryer = obj;

			this._StorageUser.QueryProvider<IAccountFinder>().Supply += this._AccountFinder;
		}

		private void _AccountFinder(IAccountFinder obj)
		{
			this._StorageUser.QueryProvider<IAccountFinder>().Supply -= this._AccountFinder;
			this._Feature.AccountFinder = obj;

			this._StorageUser.QueryProvider<ITradeNotes>().Supply += this.BuildCenterStage_Supply;
		}

		private void BuildCenterStage_Supply(ITradeNotes obj)
		{
			this._StorageUser.QueryProvider<ITradeNotes>().Supply -= this.BuildCenterStage_Supply;
			this._Feature.TradeAccount = obj;

			this._StorageUser.QueryProvider<IRecordQueriers>().Supply += this._RecordQueriers;
		}

		private void _RecordQueriers(IRecordQueriers obj)
		{
			this._StorageUser.QueryProvider<IRecordQueriers>().Supply -= this._RecordQueriers;
			this._Feature.RecordQueriers = obj;
			this.BuiledEvent(this._Feature);
		}
	}
}