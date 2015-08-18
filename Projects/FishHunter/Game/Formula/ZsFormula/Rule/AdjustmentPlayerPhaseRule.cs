// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdjustmentPlayerPhaseRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   玩家阶段起伏的调整
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;


using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     玩家阶段起伏的调整
	/// </summary>
	public class AdjustmentPlayerPhaseRule
	{
		private readonly StageDataVisitor _StageVisitor;

		public AdjustmentPlayerPhaseRule(StageDataVisitor stage_visitor)
		{
			_StageVisitor = stage_visitor;
		}

		public void Run()
		{
			if (_StageVisitor.PlayerRecord.BufferValue < 0)
			{
				_StageVisitor.PlayerRecord.Status = 0;
			}

			if (_StageVisitor.PlayerRecord.Status > 0)
			{
				_StageVisitor.PlayerRecord.Status--;
			}
			else if(_StageVisitor.Random.NextInt(0, 1000) >= 200)
			{
				// 20%
				return;
			}

			// 從VIR00 - VIR03
			var enums = EnumHelper.GetEnums<FarmBuffer.BUFFER_TYPE>().ToArray();

			for(var i = enums[(int)FarmBuffer.BUFFER_TYPE.BUFFER_VIR_BEGIN];
			    i < enums[(int)FarmBuffer.BUFFER_TYPE.BUFFER_VIR_END];
			    ++i)
			{
				var bufferData = _StageVisitor.FocusFishFarmData.FindBuffer(_StageVisitor.FocusBufferBlock, i);

				var top = bufferData.Top * bufferData.BufferTempValue.AverageValue;

				if(bufferData.Buffer <= top)
				{
					continue;
				}

				if(_StageVisitor.Random.NextInt(0, 1000) < bufferData.Gate)
				{
					bufferData.Buffer -= top;

					_StageVisitor.PlayerRecord.Status = bufferData.Top * 5;
					_StageVisitor.PlayerRecord.BufferValue = top;
					_StageVisitor.PlayerRecord.StageRecords.Find(x => x.FarmId == _StageVisitor.FocusFishFarmData.FarmId).AsnTimes += 1;
				}
				else
				{
					bufferData.Buffer = 0;
				}
			}
		}
	}
}
