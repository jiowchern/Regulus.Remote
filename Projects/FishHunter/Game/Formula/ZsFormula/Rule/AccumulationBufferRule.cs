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

			for(var i = FarmBuffer.BUFFER_TYPE.NORMAL; i < FarmBuffer.BUFFER_TYPE.COUNT; ++i)
			{
				var data = _StageVisitor.FocusFishFarmData.FindBuffer(_StageVisitor.FocusBufferBlock, i);

				_AddBufferRate(data, bet);
			}

			_StageVisitor.FocusFishFarmData.RecordData.PlayTimes += 1;
			_StageVisitor.FocusFishFarmData.RecordData.PlayTotal += bet;

			_StageVisitor.PlayerRecord.FindStageRecord(_StageVisitor.FocusFishFarmData.FarmId).PlayTimes += 1;
			_StageVisitor.PlayerRecord.FindStageRecord(_StageVisitor.FocusFishFarmData.FarmId).PlayTotal += bet;
		}

		private void _AddBufferRate(FarmBuffer data, int bet)
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
