// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccumulationBufferRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//     算法伺服器-累積buffer規則
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     算法伺服器-累積buffer規則
	/// </summary>
	public class AccumulationBufferRule
	{
		private readonly HitRequest _Request;

		private readonly StageDataVisitor _StageVisitor;

		public AccumulationBufferRule(StageDataVisitor stage_visitor, HitRequest request)
		{
			_StageVisitor = stage_visitor;
			_Request = request;

		}

		public void Run()
		{
			var bet = _Request.WeaponData.WepOdds * _Request.WeaponData.WepBet;

			for(var i = StageBuffer.BUFFER_TYPE.NORMAL; i < StageBuffer.BUFFER_TYPE.COUNT; ++i)
			{
				var data = _StageVisitor.FocusStageData.FindBuffer(_StageVisitor.FocusBufferBlock, i);

				_AddBufferRate(data, bet);
			}

			_StageVisitor.FocusStageData.RecordData.PlayTimes += 1;
			_StageVisitor.FocusStageData.RecordData.PlayTotal += bet;

			_StageVisitor.PlayerRecord.FindStageRecord(_StageVisitor.FocusStageData.StageId).PlayTimes += 1;
			_StageVisitor.PlayerRecord.FindStageRecord(_StageVisitor.FocusStageData.StageId).PlayTotal += bet;
		}

		private void _AddBufferRate(StageBuffer data, int bet)
		{
			data.Count += bet * data.Rate;
			if(data.Count < 1000)
			{
				return;
			}

			data.Buffer += data.Count / 1000;
			data.Count = data.Count % 1000;
		}
	}
}
