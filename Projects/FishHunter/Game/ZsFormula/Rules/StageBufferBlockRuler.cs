// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageBufferBlockRuler.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   stage buffer區段取得規則
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using VGame.Project.FishHunter.ZsFormula.DataStructs;

namespace VGame.Project.FishHunter.ZsFormula.Rules
{
	/// <summary>
	/// stage buffer區段取得規則
	/// </summary>
	public class StageBufferBlockRuler
	{
		private readonly StageDataTable.Data _NowStageData;

		private readonly WeaponDataTable.Data _WeaponData;

		public StageBufferBlockRuler(StageDataTable.Data now_stage_data, WeaponDataTable.Data weap_data)
		{
			_NowStageData = now_stage_data;
			_WeaponData = weap_data;
		}

		public StageDataTable.BufferData.BUFFER_BLOCK GetBufferBlock()
		{
			var bet = Bet();
			var maxBet = _NowStageData.MaxBet;

			var betIdx = StageDataTable.BufferData.BUFFER_BLOCK.BLOCK_1;
			if (bet >= ((750 * maxBet) / 1000))
			{
				betIdx = StageDataTable.BufferData.BUFFER_BLOCK.BLOCK_4;
			}
			else if (bet >= ((500 * maxBet) / 1000))
			{
				betIdx = StageDataTable.BufferData.BUFFER_BLOCK.BLOCK_3;
			}
			else if (bet >= ((250 * maxBet) / 1000))
			{
				betIdx = StageDataTable.BufferData.BUFFER_BLOCK.BLOCK_2;
			}
			else if (bet >= ((100 * maxBet) / 1000))
			{
				betIdx = StageDataTable.BufferData.BUFFER_BLOCK.BLOCK_1;
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