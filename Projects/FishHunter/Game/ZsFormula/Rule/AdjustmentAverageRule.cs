#region Test_Region

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.ZsFormula.Data;

#endregion

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	/// <summary>
	///     平均押注的调整
	/// </summary>
	public class AdjustmentAverageRule
	{
		private readonly FishStageVisitor _FishStageVisitor;

		private readonly HitRequest _HitRequest;

		public AdjustmentAverageRule(FishStageVisitor fish_stage_visitor, HitRequest hit_request)
		{
			_FishStageVisitor = fish_stage_visitor;
			_HitRequest = hit_request;
		}

		public void Run()
		{
			var bufferData = _FishStageVisitor.NowData.FindBuffer(
				_FishStageVisitor.NowBlock, 
				StageBuffer.BUFFER_TYPE.NORMAL);
			var avgBet = 0;

			// 前1000局，按照实际总玩分/总玩次，获得平均押注
			// 之后，每次减去1/100000，再补上最新的押注
			if (bufferData.BufferTempValue.AverageTimes < 1000)
			{
				bufferData.BufferTempValue.AverageTimes += 1;

				bufferData.BufferTempValue.AverageTotal += _HitRequest.WeaponData.WepBet;

				avgBet = bufferData.BufferTempValue.AverageTotal / bufferData.BufferTempValue.AverageTimes;

				if (bufferData.BufferTempValue.AverageTimes == 1000)
				{
					bufferData.BufferTempValue.AverageTotal = bufferData.BufferTempValue.AverageTotal / 1000 * 100000;
				}
			}
			else
			{
				bufferData.BufferTempValue.AverageTotal -= bufferData.BufferTempValue.AverageTotal / 100000;
				bufferData.BufferTempValue.AverageTotal += _HitRequest.WeaponData.WepBet;
				avgBet = bufferData.BufferTempValue.AverageTotal / 100000;
			}

			bufferData.BufferTempValue.AverageValue = avgBet;
		}
	}
}