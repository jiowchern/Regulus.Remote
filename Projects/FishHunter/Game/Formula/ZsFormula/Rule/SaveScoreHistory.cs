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

		private readonly int _WinScore;

		public SaveScoreHistory(DataVisitor data_visitor, int win_Score)
		{
			_DataVisitor = data_visitor;
			_WinScore = win_Score;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Run()
		{
			if(_WinScore == 0)
			{
				return;
			}

			_SaveHitWinScore();

			_SaveBlockScore();

			_SavePlayerLuck();
		}

		private void _SaveHitWinScore()
		{
			_DataVisitor.Farm.Record.WinScore += _WinScore;

			_DataVisitor.PlayerRecord.FindFarmRecord(_DataVisitor.Farm.FarmId).WinScore += _WinScore;
		}

		private void _SaveBlockScore()
		{
			var farmDataRoot = _DataVisitor.Farm.FindDataRoot(
				_DataVisitor.FocusBlockName, 
				FarmDataRoot.BufferNode.BUFFER_NAME.NORMAL);

			farmDataRoot.Block.WinScore += _WinScore;
			farmDataRoot.Buffer.WinScore -= _WinScore;
		}


		private void _SavePlayerLuck()
		{
			// 玩家阶段起伏的调整
			if (_DataVisitor.PlayerRecord.Status <= 0)
			{
				return;
			}

			_DataVisitor.PlayerRecord.BufferValue -= _WinScore;

			_DataVisitor.PlayerRecord.FindFarmRecord(_DataVisitor.Farm.FarmId).AsnWin += _WinScore;
		}
	}
}
