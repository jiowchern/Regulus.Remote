// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdjustmentGameLevelRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the AdjustmentGameLevelRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using VGame.Project.FishHunter.Common.Datas.FishStage;
using VGame.Project.FishHunter.ZsFormula.Data;

#endregion

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	public class AdjustmentGameLevelRule
	{
		private readonly StageDataVisit _StageDataVisit;

		public AdjustmentGameLevelRule(StageDataVisit stage_data_visit1)
		{
			_StageDataVisit = stage_data_visit1;
		}

		/// <summary>
		///     游戏难度的调整
		/// </summary>
		public void Run()
		{
			var bufferData = _StageDataVisit.FindBuffer(
				_StageDataVisit.NowUseBlock, 
				StageBuffer.BUFFER_TYPE.NORMAL);
			var bufferTemp = bufferData.BufferTempValue;
			bufferTemp.PlayerTime += 1;
			if (bufferTemp.RealTime != 0 && bufferTemp.PlayerTime <= 500)
			{
				return;
			}

			// 一分钟到了，就微调一次
			bufferTemp.RealTime = 60; // sec

			// 或是已经发射500发子弹了，就微调一次
			bufferTemp.PlayerTime -= 500;

			var baseValue = _StageDataVisit.NowUseData.NowBaseOdds * bufferTemp.AverageValue;

			// TODO:
			bufferTemp.HiLoRate = new NatureDataRule().Run(bufferData.Buffer, baseValue);
		}
	}
}