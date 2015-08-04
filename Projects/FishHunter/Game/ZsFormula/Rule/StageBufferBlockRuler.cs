// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageBufferBlockRuler.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   stage buffer區段取得規則
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using VGame.Project.FishHunter.Common.Datas.FishStage;
using VGame.Project.FishHunter.ZsFormula.Data;

#endregion

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	/// <summary>
	///     stage buffer區段取得規則
	/// </summary>
	public class StageBufferBlockRuler
	{
		private readonly StageData _NowStageData;

		private readonly WeaponDataTable.Data _WeaponData;

		public StageBufferBlockRuler(StageData now_stage_data, WeaponDataTable.Data weap_data)
		{
			_NowStageData = now_stage_data;
			_WeaponData = weap_data;
		}

		public StageBuffer.BUFFER_BLOCK GetBufferBlock()
		{
			var bet = Bet();
			var maxBet = _NowStageData.MaxBet;

			var betIdx = StageBuffer.BUFFER_BLOCK.BLOCK_1;
			if (bet >= ((750 * maxBet) / 1000))
			{
				betIdx = StageBuffer.BUFFER_BLOCK.BLOCK_4;
			}
			else if (bet >= ((500 * maxBet) / 1000))
			{
				betIdx = StageBuffer.BUFFER_BLOCK.BLOCK_3;
			}
			else if (bet >= ((250 * maxBet) / 1000))
			{
				betIdx = StageBuffer.BUFFER_BLOCK.BLOCK_2;
			}
			else if (bet >= ((100 * maxBet) / 1000))
			{
				betIdx = StageBuffer.BUFFER_BLOCK.BLOCK_1;
			}

			return betIdx;
		}

		private int Bet()
		{
			var bet = _WeaponData.WeaponOdds * _WeaponData.WeaponBet;
			return bet;
		}
	}
}