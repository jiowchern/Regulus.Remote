﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccumulationBufferRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   累積buffer
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using VGame.Project.FishHunter.ZsFormula.DataStructs;

namespace VGame.Project.FishHunter.ZsFormula.Rules
{
	/// <summary>
	/// 累積buffer
	/// </summary>
	public class AccumulationBufferRule
	{
		private readonly StageDataVisit _StageDataVisit;

		public AccumulationBufferRule(StageDataVisit stage_data_visit)
		{
			_StageDataVisit = stage_data_visit;
		}

		public void Run(int bet, Player.Data player_data)
		{
			for (var i = StageDataTable.BufferData.BUFFER_TYPE.NORMAL; i < StageDataTable.BufferData.BUFFER_TYPE.COUNT; ++i)
			{
				var data = _StageDataVisit.FindBufferData(_StageDataVisit.NowUseBlock, i);

				_AddBufferRate(data, bet);
			}

			_StageDataVisit.NowUseData.Recode.PlayTimes += 1;
			_StageDataVisit.NowUseData.Recode.PlayTotal += bet;

			player_data.Recode.PlayTimes += 1;
			player_data.Recode.PlayTotal += bet;
		}

		private void _AddBufferRate(StageDataTable.BufferData data, int bet)
		{
			data.Count += bet * data.Rate;
			if (data.Count >= 1000)
			{
				data.Buffer += data.Count / 1000;
				data.Count = data.Count % 1000;
			}
		}
	}
}