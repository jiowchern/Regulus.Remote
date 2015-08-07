using System.Linq;


using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.ZsFormula.Data;

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	/// <summary>
	///     玩家阶段起伏的调整
	/// </summary>
	public class AdjustmentPlayerPhaseRule
	{
		private readonly PlayerRecord _PlayerRecord;

		private readonly FishStageVisitor _StageVisitor;

		private static int RandNumber
		{
			get { return Random.Instance.NextInt(0, 1000); }
		}

		public AdjustmentPlayerPhaseRule(FishStageVisitor stage_visitor, PlayerRecord player_record)
		{
			_StageVisitor = stage_visitor;
			_PlayerRecord = player_record;
		}

		public void Run()
		{
			if (_PlayerRecord.BufferValue < 0)
			{
				_PlayerRecord.Status = 0;
			}

			if (_PlayerRecord.Status > 0)
			{
				_PlayerRecord.Status--;
			}
			else if (AdjustmentPlayerPhaseRule.RandNumber >= 200)
			{
				// 20%
				return;
			}

			// 從VIR00 - VIR03
			var enums = EnumHelper.GetEnums<StageBuffer.BUFFER_TYPE>().ToArray();

			for (var i = enums[(int)StageBuffer.BUFFER_TYPE.BUFFER_VIR_BEGIN];
			     i < enums[(int)StageBuffer.BUFFER_TYPE.BUFFER_VIR_END]; ++i)
			{
				var bufferData = _StageVisitor.NowData.FindBuffer(_StageVisitor.NowBlock, i);

				var top = bufferData.Top * bufferData.BufferTempValue.AverageValue;

				if (bufferData.Buffer <= top)
				{
					continue;
				}

				if (AdjustmentPlayerPhaseRule.RandNumber < bufferData.Gate)
				{
					bufferData.Buffer -= top;
					_PlayerRecord.Status = (int)(bufferData.Top * 5);
					_PlayerRecord.BufferValue = (int)top;

					_PlayerRecord.StageRecord.Find(x => x.StageId == _StageVisitor.NowData.StageId).AsnTimes += 1;
				}
				else
				{
					bufferData.Buffer = 0;
				}
			}
		}
	}
}