#region Test_Region

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.ZsFormula.Data;

#endregion

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	public class AdjustmentGameLevelRule
	{
		private readonly FishStageVisitor _StageVisitor;

		public AdjustmentGameLevelRule(FishStageVisitor stage_visitor)
		{
			_StageVisitor = stage_visitor;
		}

		/// <summary>
		///     游戏难度的调整
		/// </summary>
		public void Run()
		{
			var bufferData = _StageVisitor.NowData.FindBuffer(_StageVisitor.NowBlock, StageBuffer.BUFFER_TYPE.NORMAL);
			var bufferTemp = bufferData.BufferTempValue;
			bufferTemp.FireCount += 1;
			if (bufferTemp.RealTime != 0 && bufferTemp.FireCount <= 500)
			{
				return;
			}

			// 一分钟到了，就微调一次
			bufferTemp.RealTime = 60; // sec

			// 或是已经发射500发子弹了，就微调一次
			bufferTemp.FireCount -= 500;

			var baseValue = _StageVisitor.NowData.NowBaseOdds * bufferTemp.AverageValue;

			// TODO:
			bufferTemp.HiLoRate = new NatureDataRule().Run(bufferData.Buffer, baseValue);
		}
	}
}