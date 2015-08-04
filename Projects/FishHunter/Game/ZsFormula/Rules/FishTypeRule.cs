// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FishTypeRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the FishTypeRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using VGame.Project.FishHunter.ZsFormula.DataStructs;

namespace VGame.Project.FishHunter.ZsFormula.Rules
{
	/// <summary>
	/// 記下特殊魚拿到幾次
	/// </summary>
	public class FishTypeRule
	{
		private readonly StageDataVisit _StageDataVisit;

		public FishTypeRule(StageDataVisit stage_data_visit)
		{
			_StageDataVisit = stage_data_visit;
		}

		public void Run(AttackData attack_data, Player.Data player_data)
		{
			if (attack_data.FishData.FishType < FishDataTable.Data.FISH_TYPE.DEF_100)
			{
				return;
			}

			// 沒用過的就可以
			// 存玩家的
			var data = player_data.RecodeData.SpecialWeaponDatas.Find(x => x.IsUsed == false);
			data.SpId = (int)attack_data.FishData.FishType;
			data.WinFrequency++;

			// 存stage的
			var stageData = _StageDataVisit.NowUseData.RecodeData.SpecialWeaponDatas.Find(x => x.IsUsed == false);
			stageData.SpId = (int)attack_data.FishData.FishType;
			stageData.WinFrequency++;
		}
	}
}