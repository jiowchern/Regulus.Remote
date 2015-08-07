using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.ZsFormula.Data;

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	/// <summary>
	///     分數記錄
	/// </summary>
	public class DiedHandleRule
	{
		private readonly FishStageVisitor _FishStageVisitor;

		private readonly PlayerRecord _PlayerRecord;

		private readonly int _Win;

		public DiedHandleRule(FishStageVisitor fish_stage_visitor, PlayerRecord player_record, int win)
		{
			_FishStageVisitor = fish_stage_visitor;
			_PlayerRecord = player_record;
			_Win = win;
		}

		/// <summary>
		/// </summary>
		public void Run()
		{
			if (_Win == 0)
			{
				return;
			}

			var bufferData = _FishStageVisitor.NowData.FindBuffer(_FishStageVisitor.NowBlock, StageBuffer.BUFFER_TYPE.NORMAL);

			bufferData.Buffer -= _Win;

			_FishStageVisitor.NowData.RecordData.WinFrequency += 1;
			_FishStageVisitor.NowData.RecordData.WinScore += _Win;

			_PlayerRecord.StageRecord.Find(x => x.StageId == _FishStageVisitor.NowData.StageId).WinFrequency += 1;
			_PlayerRecord.StageRecord.Find(x => x.StageId == _FishStageVisitor.NowData.StageId).WinScore += 1;

			// 玩家阶段起伏的调整
			if (_PlayerRecord.Status <= 0)
			{
				return;
			}

			_PlayerRecord.BufferValue -= _Win;

			_PlayerRecord.StageRecord.Find(x => x.StageId == _FishStageVisitor.NowData.StageId).AsnWin += _Win;
		}
	}
}