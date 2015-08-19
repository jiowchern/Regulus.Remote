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
		private readonly FarmDataVisitor _FarmVisitor;

		public AdjustmentPlayerPhaseRule(FarmDataVisitor farm_visitor)
		{
			_FarmVisitor = farm_visitor;
		}

		public void Run()
		{
			if (_FarmVisitor.PlayerRecord.BufferValue < 0)
			{
				_FarmVisitor.PlayerRecord.Status = 0;
			}

			if (_FarmVisitor.PlayerRecord.Status > 0)
			{
				_FarmVisitor.PlayerRecord.Status--;
			}
			else if(_FarmVisitor.Random.NextInt(0, 1000) >= 200)
			{
				// 20%
				return;
			}

			// 從VIR00 - VIR03
		    var enums =
		        EnumHelper.GetEnums<FarmBuffer.BUFFER_TYPE>()
		                  .Where(x => x >= FarmBuffer.BUFFER_TYPE.VIR00 && x <= FarmBuffer.BUFFER_TYPE.VIR03);

            foreach (var i in enums)
			{
				var bufferData = _FarmVisitor.FocusFishFarmData.FindBuffer(_FarmVisitor.FocusBufferBlock, i);

				var top = bufferData.Top * bufferData.BufferTempValue.AverageValue;

				if(bufferData.Buffer <= top)
				{
					continue;
				}

				if(_FarmVisitor.Random.NextInt(0, 1000) < bufferData.Gate)
				{
					bufferData.Buffer -= top;

					_FarmVisitor.PlayerRecord.Status = bufferData.Top * 5;
					_FarmVisitor.PlayerRecord.BufferValue = top;
					_FarmVisitor.PlayerRecord.StageRecords.First(x => x.FarmId == _FarmVisitor.FocusFishFarmData.FarmId).AsnTimes += 1;
				}
				else
				{
					bufferData.Buffer = 0;
				}
			}
		}
	}
}
