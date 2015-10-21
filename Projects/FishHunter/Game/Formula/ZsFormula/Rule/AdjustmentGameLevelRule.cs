using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


using NLog;
using NLog.Fluent;


using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	public class AdjustmentGameLevelRule
	{
		private struct Data
		{
			public int BufferValue;

			public string Name;

			public Data(string name, int buffer_value)
			{
				Name = name;
				BufferValue = buffer_value;
			}
		}

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
			var bufferData = _Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName, FarmDataRoot.BufferNode.BUFFER_NAME.NORMAL);
			var bufferTemp = bufferData.TempValueNode;
			bufferTemp.FireCount += 1;

			// 一分內 未遠500發子彈
			if(_TimeCounter.Second > bufferTemp.RealTime && bufferTemp.FireCount < 500)
			{
				_Update();
			}

			if(bufferTemp.FireCount >= 500)
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
			bufferTemp.FireCount -= 500;
		}

		private FarmDataRoot.ValueNode _SetHiLoRate()
		{
			var bufferData = _Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName, FarmDataRoot.BufferNode.BUFFER_NAME.NORMAL);

			var bufferTemp = bufferData.TempValueNode;

			var baseValue = _Visitor.Farm.NowBaseOdds * bufferTemp.AverageValue;

			bufferTemp.HiLoRate = new NatureDataRule().Run(bufferData.Buffer.WinScore, baseValue);

			return bufferTemp;
		}
	}
}
