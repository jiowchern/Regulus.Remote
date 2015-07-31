// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecialItemRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   取得道具
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using VGame.Project.FishHunter.ZsFormula.DataStructs;

namespace VGame.Project.FishHunter.ZsFormula.Rules
{
	/// <summary>
	/// 取得道具
	/// </summary>
	public class SpecialItemRule
	{
		private readonly AttackData _AttackData;

		private readonly StageDataVisit _StageDataVisit;

		public SpecialItemRule(StageDataVisit stage_data_visit)
		{
			_StageDataVisit = stage_data_visit;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="player_data"></param>
		/// <returns></returns>
		public int Run(Player.Data player_data)
		{
			var item = player_data.Item;
			if (item == 0)
			{
				return item;
			}

			player_data.Item = 0;

			// TODO index操作有問題
			player_data.Recode.Sp00WinTimes++;
			_StageDataVisit.NowUseData.Recode.Sp00WinTimes++;

			return item;
		}
	}
}