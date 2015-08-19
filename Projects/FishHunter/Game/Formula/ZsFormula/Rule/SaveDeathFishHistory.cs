// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveDeathFishHistory.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   �O���D����o����
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.Linq;
using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	/// �O���S������o����
	/// </summary>
	public class SaveDeathFishHistory
	{
		private readonly RequsetFishData _Fish;

		private readonly FarmDataVisitor _FarmVisitor;

		public SaveDeathFishHistory(FarmDataVisitor farm_visitor, RequsetFishData fish)
		{
			_Fish = fish;
			_FarmVisitor = farm_visitor;
		}

		public void Run()
		{
			// �splayer��������
			_FarmVisitor.PlayerRecord.FindFarmRecord(_FarmVisitor.FocusFishFarmData.FarmId)
			            .FishHitReuslt.Items.First(x => x.FishType == _Fish.FishType).KillCount++;
			             
			// �sstage�����`����
			_FarmVisitor.FocusFishFarmData.RecordData.FishHitReuslt.Items.First(x => x.FishType == _Fish.FishType).KillCount++;
		}
	}
}