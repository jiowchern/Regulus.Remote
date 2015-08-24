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
		private readonly DataVisitor _DataVisitor;

		private readonly int _Win;

		public SaveScoreHistory(DataVisitor data_visitor, int win)
		{
			_DataVisitor = data_visitor;
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
			var bufferData = _DataVisitor.Farm.FindBuffer(
				_DataVisitor.FocusBufferBlock,
				FarmBuffer.BUFFER_TYPE.NORMAL);

			bufferData.Buffer -= _Win;

			_DataVisitor.Farm.Record.WinScore += _Win;
		}

		private void _SavePlayerScore()
		{
			// 玩家阶段起伏的调整
			if(_DataVisitor.PlayerRecord.Status <= 0)
			{
				return;
			}
			_DataVisitor.PlayerRecord.BufferValue -= _Win;

			_DataVisitor.PlayerRecord.FarmRecords.First(x => x.FarmId == _DataVisitor.Farm.FarmId).AsnWin
				+= _Win;
		}
	}
}
