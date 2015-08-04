// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccumulationBufferRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   累積buffer
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using VGame.Project.FishHunter.Common.Datas.FishStage;
using VGame.Project.FishHunter.ZsFormula.Data;

#endregion

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	/// <summary>
	///     累積buffer
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
			for (var i = StageBuffer.BUFFER_TYPE.NORMAL; i < StageBuffer.BUFFER_TYPE.COUNT; ++i)
			{
				var data = _StageDataVisit.FindBuffer(_StageDataVisit.NowUseBlock, i);

				_AddBufferRate(data, bet);
			}

			_StageDataVisit.NowUseData.RecodeData.PlayTimes += 1;
			_StageDataVisit.NowUseData.RecodeData.PlayTotal += bet;

			player_data.RecodeData.PlayTimes += 1;
			player_data.RecodeData.PlayTotal += bet;
		}

		private void _AddBufferRate(StageBuffer data, int bet)
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