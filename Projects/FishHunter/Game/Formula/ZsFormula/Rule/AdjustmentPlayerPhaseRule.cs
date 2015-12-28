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

			// 從VIR00 - VIR03
			CheckFarmBufferType();
		}

		private void CheckFarmBufferType()
		{
			var enums = EnumHelper.GetEnums<FarmDataRoot.BufferNode.BUFFER_NAME>()
								.Where(x =>
										x >= FarmDataRoot.BufferNode.BUFFER_NAME.VIR00 &&
										x <= FarmDataRoot.BufferNode.BUFFER_NAME.VIR03);

			foreach(var i in enums)
			{
				var farmDataRoot = _Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName, i);

				var normal = _Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName,
														FarmDataRoot.BufferNode.BUFFER_NAME.NORMAL);

				var top = farmDataRoot.Buffer.Top * normal.TempValueNode.AverageValue;

				if(farmDataRoot.Buffer.WinScore <= top)
				{
					continue;
				}

				var randomValue = _Visitor.FindIRandom(RandomData.RULE.ADJUSTMENT_PLAYER_PHASE, 1)
										.NextInt(0, 1000);

				if(randomValue < farmDataRoot.Buffer.Gate)
				{
					farmDataRoot.Buffer.WinScore -= top;

					_Visitor.PlayerRecord.Status = farmDataRoot.Buffer.Top * 5;
					_Visitor.PlayerRecord.BufferValue = top;
					_Visitor.PlayerRecord.FindFarmRecord(_Visitor.Farm.FarmId).AsnTimes += 1;
				}
				else
				{
					farmDataRoot.Buffer.WinScore = 0;
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
