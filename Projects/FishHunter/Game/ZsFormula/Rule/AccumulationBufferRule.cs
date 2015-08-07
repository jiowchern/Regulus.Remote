
using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.ZsFormula.Data;

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	/// <summary>
	///     累積buffer
	/// </summary>
	public class AccumulationBufferRule
	{
		private readonly PlayerRecord _PlayerRecord;

		private readonly HitRequest _Request;

		private readonly FishStageVisitor _StageVisitor;

		public AccumulationBufferRule(FishStageVisitor stage_visitor, HitRequest request, PlayerRecord player_record)
		{
			_StageVisitor = stage_visitor;
			_Request = request;
			_PlayerRecord = player_record;
		}


		public void Run()
		{
			for (var i = StageBuffer.BUFFER_TYPE.NORMAL; i < StageBuffer.BUFFER_TYPE.COUNT; ++i)
			{
				var data = _StageVisitor.NowData.FindBuffer(_StageVisitor.NowBlock, i);

				_AddBufferRate(data, _Request.WeaponData.WepBet);
			}

			_StageVisitor.NowData.RecordData.PlayTimes += 1;
			_StageVisitor.NowData.RecordData.PlayTotal += _Request.WeaponData.WepBet;

			_PlayerRecord.FindStageRecord(_StageVisitor.NowData.StageId).PlayTimes += 1;
			_PlayerRecord.FindStageRecord(_StageVisitor.NowData.StageId).PlayTotal += _Request.WeaponData.WepBet;
		}

		private void _AddBufferRate(StageBuffer data, int bet)
		{
			data.Count += bet * data.Rate;
			if (data.Count >= 1000)
			{
				data.Buffer += data.Count / 1000;
				data.Count = data.Count % 1000;
			}
		}
	}
}