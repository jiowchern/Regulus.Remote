using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	public class AdjustmentGameLevelRule
	{
		private const int _CheckNumbers = 100;

		private readonly TimeCounter _TimeCounter;

		private readonly DataVisitor _Visitor;

		public AdjustmentGameLevelRule(DataVisitor visitor)
		{
			_Visitor = visitor;
			_TimeCounter = new TimeCounter();
		}

		/// <summary>
		///     游戏难度的调整
		/// </summary>
		public void Run()
		{
			var normal = _Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName,
														FarmDataRoot.BufferNode.BUFFER_NAME.NORMAL);
			var bufferTemp = normal.TempValueNode;

			bufferTemp.FireCount += 1;

			// 一分內 未遠500發子彈
			if(_TimeCounter.Second > bufferTemp.RealTime &&
				bufferTemp.FireCount < AdjustmentGameLevelRule._CheckNumbers)
			{
				_Update();
			}

			if(bufferTemp.FireCount >= AdjustmentGameLevelRule._CheckNumbers)
			{
				_Update2();
			}
		}

		private void _Update()
		{
			_SetHiLoRate();

			_TimeCounter.Reset();
		}

		private void _Update2()
		{
			var bufferTemp = _SetHiLoRate();
			bufferTemp.FireCount -= AdjustmentGameLevelRule._CheckNumbers;
		}

		private FarmDataRoot.ValueNode _SetHiLoRate()
		{
			var normal = _Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName,
														FarmDataRoot.BufferNode.BUFFER_NAME.NORMAL);

			var bufferTemp = normal.TempValueNode;

			var baseValue = _Visitor.Farm.NowBaseOdds * bufferTemp.AverageValue;

			bufferTemp.HiLoRate = new NatureDataRule().Run(normal.Buffer.WinScore, baseValue);

			return bufferTemp;
		}
	}
}
