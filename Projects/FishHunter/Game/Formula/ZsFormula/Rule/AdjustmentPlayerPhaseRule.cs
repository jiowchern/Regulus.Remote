
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
			var randomValue = _Visitor.FindIRandom(RandomData.RULE.ADJUSTMENT_PLAYER_PHASE, 0).NextInt(0, 1000);

			if(_CheckPlayerRecord(randomValue))
			{
				return;
			}

			randomValue = _Visitor.FindIRandom(RandomData.RULE.ADJUSTMENT_PLAYER_PHASE, 1).NextInt(0, 1000);

			// 從VIR00 - VIR03
			CheckFarmBufferType(randomValue);
		}

		private void CheckFarmBufferType(int random_value)
		{
			var enums =
				EnumHelper.GetEnums<FarmBuffer.BUFFER_TYPE>()
						.Where(x => x >= FarmBuffer.BUFFER_TYPE.VIR00 && x <= FarmBuffer.BUFFER_TYPE.VIR03);

			foreach(var i in enums)
			{
				var bufferData = _Visitor.Farm.FindBuffer(_Visitor.FocusBufferBlock, i);

				var top = bufferData.Top * bufferData.BufferTempValue.AverageValue;

				if(bufferData.Buffer <= top)
				{
					continue;
				}

				if(random_value < bufferData.Gate)
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

		private bool _CheckPlayerRecord(int random)
		{
			if(_Visitor.PlayerRecord.BufferValue < 0)
			{
				_Visitor.PlayerRecord.Status = 0;
			}

			if(_Visitor.PlayerRecord.Status > 0)
			{
				_Visitor.PlayerRecord.Status--;
			}
			else if(random >= 200)
			{
				// 20%
				return true;
			}

			return false;
		}
	}
}
