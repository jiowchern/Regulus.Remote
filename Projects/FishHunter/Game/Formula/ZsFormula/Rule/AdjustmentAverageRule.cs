// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdjustmentAverageRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   平均押注的调整
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     平均押注的调整
	/// </summary>
	public class AdjustmentAverageRule
	{
		private readonly FarmDataVisitor _FarmDataVisitor;

		private readonly HitRequest _HitRequest;

		public AdjustmentAverageRule(FarmDataVisitor fish_farm_visitor, HitRequest hit_request)
		{
			_FarmDataVisitor = fish_farm_visitor;
			_HitRequest = hit_request;
		}

		public void Run()
		{
			var bufferData = _FarmDataVisitor.FocusFishFarmData.FindBuffer(
				_FarmDataVisitor.FocusBufferBlock, 
				FarmBuffer.BUFFER_TYPE.NORMAL);

			var bet = _HitRequest.WeaponData.WepOdds * _HitRequest.WeaponData.WepBet;

			// 前1000局，按照实际总玩分/总玩次，获得平均押注
			// 之后，每次减去1/100000，再补上最新的押注
			if(bufferData.BufferTempValue.AverageTimes < 1000)
			{
				bufferData.BufferTempValue.AverageTimes += 1;

				bufferData.BufferTempValue.AverageTotal += bet;

				bufferData.BufferTempValue.AverageValue = bufferData.BufferTempValue.AverageTotal / bufferData.BufferTempValue.AverageTimes;

				if(bufferData.BufferTempValue.AverageTimes == 1000)
				{
					bufferData.BufferTempValue.AverageTotal = bufferData.BufferTempValue.AverageTotal / 1000 * 100000;
				}
			}
			else
			{
				bufferData.BufferTempValue.AverageTotal -= bufferData.BufferTempValue.AverageTotal / 100000;
				bufferData.BufferTempValue.AverageTotal += bet;
				bufferData.BufferTempValue.AverageValue = bufferData.BufferTempValue.AverageTotal / 100000;
			}
		}
	}
}
