// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveScoreHistory.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   分數記錄
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     分數記錄
	/// </summary>
	public class SaveScoreHistory
	{
		private readonly FarmDataVisitor _FarmDataVisitor;

		private readonly int _Win;

		public SaveScoreHistory(FarmDataVisitor fish_farm_visitor, int win)
		{
			_FarmDataVisitor = fish_farm_visitor;
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
			var bufferData = _FarmDataVisitor.FocusFishFarmData.FindBuffer(
				_FarmDataVisitor.FocusBufferBlock,
				FarmBuffer.BUFFER_TYPE.NORMAL);

			bufferData.Buffer -= _Win;

			_FarmDataVisitor.FocusFishFarmData.RecordData.WinScore += _Win;
		}

		private void _SavePlayerScore()
		{
			// 玩家阶段起伏的调整
			if(_FarmDataVisitor.PlayerRecord.Status <= 0)
			{
				return;
			}
			_FarmDataVisitor.PlayerRecord.BufferValue -= _Win;

			_FarmDataVisitor.PlayerRecord.StageRecords.First(x => x.FarmId == _FarmDataVisitor.FocusFishFarmData.FarmId).AsnWin
				+= _Win;
		}
	}
}
