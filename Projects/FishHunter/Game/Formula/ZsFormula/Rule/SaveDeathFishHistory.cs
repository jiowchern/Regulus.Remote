// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveDeathFishHistory.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   記錄道具獲得次數
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.Linq;
using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	/// 記錄特殊魚獲得次數
	/// </summary>
	public class SaveDeathFishHistory
	{
		private readonly RequsetFishData _Fish;

		private readonly StageDataVisitor _StageVisitor;

		public SaveDeathFishHistory(StageDataVisitor stage_visitor, RequsetFishData fish)
		{
			_Fish = fish;
			_StageVisitor = stage_visitor;
		}

		public void Run()
		{
			// 存player打死的魚
			_StageVisitor.PlayerRecord.FindStageRecord(_StageVisitor.FocusFishFarmData.FarmId)
			             .FishHitReuslt.Items.Where(x => x.FishType == _Fish.FishType).First().KillCount++;
			             
			// 存stage的死亡的魚
			_StageVisitor.FocusFishFarmData.RecordData.FishHitReuslt.Items.Where(x => x.FishType == _Fish.FishType).First().KillCount++;
		}
	}
}
