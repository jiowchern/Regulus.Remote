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
		private readonly DataVisitor _Visitor;

		public AdjustmentPlayerPhaseRule(DataVisitor visitor)
		{
			_Visitor = visitor;
		}

		public void Run()
		{
			if (_Visitor.PlayerRecord.BufferValue < 0)
			{
				_Visitor.PlayerRecord.Status = 0;
			}

			if (_Visitor.PlayerRecord.Status > 0)
			{
				_Visitor.PlayerRecord.Status--;
			}
			else if(_Visitor.Random.NextInt(0, 1000) >= 200)
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
				var bufferData = _Visitor.Farm.FindBuffer(_Visitor.FocusBufferBlock, i);

				var top = bufferData.Top * bufferData.BufferTempValue.AverageValue;

				if(bufferData.Buffer <= top)
				{
					continue;
				}

				if(_Visitor.Random.NextInt(0, 1000) < bufferData.Gate)
				{
					bufferData.Buffer -= top;

					_Visitor.PlayerRecord.Status = bufferData.Top * 5;
					_Visitor.PlayerRecord.BufferValue = top;
					_Visitor.PlayerRecord.FarmRecords.First(x => x.FarmId == _Visitor.Farm.FarmId).AsnTimes += 1;
				}
				else
				{
					bufferData.Buffer = 0;
				}
			}
		}
	}
}
