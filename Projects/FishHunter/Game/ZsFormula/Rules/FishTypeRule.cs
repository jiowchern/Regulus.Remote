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
	public class FishTypeRule
	{
		private readonly StageDataVisit _StageDataVisit;

		public FishTypeRule(StageDataVisit stage_data_visit)
		{
			_StageDataVisit = stage_data_visit;
		}

		public void Run(AttackData attack_data, Player.Data player_data)
		{
			if (attack_data.FishData.FishType >= FishDataTable.Data.FISH_TYPE.DEF_100)
			{
				var type = attack_data.FishData.FishType - 100;

				if (type < FishDataTable.Data.FISH_TYPE.TYPE_7)
				{
					// TODO index°ÝÃD
					player_data.Recode.Sp03WinTimes++;

					_StageDataVisit.NowUseData.Recode.Sp03WinTimes++;
				}
			}
		}
	}
}