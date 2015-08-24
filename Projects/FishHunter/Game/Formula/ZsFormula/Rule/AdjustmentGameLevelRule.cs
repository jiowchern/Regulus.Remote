// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdjustmentGameLevelRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the AdjustmentGameLevelRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	public class AdjustmentGameLevelRule
	{
		private readonly DataVisitor _Visitor;

		private readonly TimeCounter _TimeCounter;

		public AdjustmentGameLevelRule(DataVisitor visitor)
		{
			_Visitor = visitor;
			_TimeCounter = new TimeCounter();
		}

		/// <summary>
		///		游戏难度的调整
		/// </summary>

		public void Run()
		{
			var bufferData = _Visitor.Farm.FindBuffer(_Visitor.FocusBufferBlock, FarmBuffer.BUFFER_TYPE.NORMAL);
			var bufferTemp = bufferData.BufferTempValue;
			bufferTemp.FireCount += 1;

			// 一分內 未遠500發子彈
			if (_TimeCounter.Second > bufferTemp.RealTime && bufferTemp.FireCount < 500)
			{
				_Update();
			}

			// 達500子彈 馬上
			if(bufferTemp.FireCount >= 500)
			{
				_Update2();
			}
		}

		private void _Update()
		{
			var bufferData = _Visitor.Farm.FindBuffer(_Visitor.FocusBufferBlock, FarmBuffer.BUFFER_TYPE.NORMAL);
			
			var bufferTemp = bufferData.BufferTempValue;
			
			var baseValue = _Visitor.Farm.NowBaseOdds * bufferTemp.AverageValue;
			
			bufferTemp.HiLoRate = new NatureDataRule().Run(bufferData.Buffer, baseValue);

			_TimeCounter.Reset();
		}

		private void _Update2()
		{
			var bufferData = _Visitor.Farm.FindBuffer(_Visitor.FocusBufferBlock, FarmBuffer.BUFFER_TYPE.NORMAL);

			var bufferTemp = bufferData.BufferTempValue;

			var baseValue = _Visitor.Farm.NowBaseOdds * bufferTemp.AverageValue;

			bufferTemp.HiLoRate = new NatureDataRule().Run(bufferData.Buffer, baseValue);

			bufferTemp.FireCount -= 500;	
		}
	}
}
