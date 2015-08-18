// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveScoreHistory.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   分數記錄
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     分數記錄
	/// </summary>
	public class SaveScoreHistory
	{
		private readonly StageDataVisitor _StageDataVisitor;

		private readonly int _Win;

		public SaveScoreHistory(StageDataVisitor fish_stage_visitor, int win)
		{
			_StageDataVisitor = fish_stage_visitor;
			_Win = win;
		}

		/// <summary>
		/// </summary>
		public void Run()
		{
			if(_Win == 0)
			{
				return;
			}

			_SaveStageScore();

			_SavePlayerScore();
		}

		private void _SaveStageScore()
		{
			var bufferData = _StageDataVisitor.FocusFishFarmData.FindBuffer(
				_StageDataVisitor.FocusBufferBlock,
				FarmBuffer.BUFFER_TYPE.NORMAL);

			bufferData.Buffer -= _Win;

			_StageDataVisitor.FocusFishFarmData.RecordData.WinScore += _Win;
		}

		private void _SavePlayerScore()
		{
			// 玩家阶段起伏的调整
			if(_StageDataVisitor.PlayerRecord.Status <= 0)
			{
				return;
			}
			_StageDataVisitor.PlayerRecord.BufferValue -= _Win;

			_StageDataVisitor.PlayerRecord.StageRecords.Find(x => x.FarmId == _StageDataVisitor.FocusFishFarmData.FarmId).AsnWin
				+= _Win;
		}
	}
}
