using System.Linq;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     記錄擊殺魚的資料
	/// </summary>
	public class SaveDeathFishHistory
	{
		private readonly DataVisitor _DataVisitor;

		private readonly RequsetFishData _Fish;

		private readonly int _WinScore;

		public SaveDeathFishHistory(DataVisitor data_visitor, RequsetFishData fish, int win_score)
		{
			_Fish = fish;
			_WinScore = win_score;
			_DataVisitor = data_visitor;
		}

		public void Run()
		{
			_SavePlayerHit();
			_SaveFarmHit();
		}

		private void _SavePlayerHit()
		{
			var hitRecords = _DataVisitor.PlayerRecord
										.FindFarmRecord(_DataVisitor.Farm.FarmId)
										.FishHits.ToList();

			var data = hitRecords.FirstOrDefault(x => x.FishType == _Fish.FishType);

			if(data == null)
			{
				data = new FishHitRecord(_Fish.FishType);
				hitRecords.Add(data);
				_DataVisitor.PlayerRecord
							.FindFarmRecord(_DataVisitor.Farm.FarmId)
							.FishHits = hitRecords.ToArray();
			}

			data.KillCount++;
			data.WinScore += _WinScore;
		}

		private void _SaveFarmHit()
		{
			var hitRecords = _DataVisitor.Farm.Record.FishHits.ToList();

			var data = hitRecords.FirstOrDefault(x => x.FishType == _Fish.FishType);

			if(data == null)
			{
				data = new FishHitRecord(_Fish.FishType);
				hitRecords.Add(data);
				_DataVisitor.Farm.Record.FishHits = hitRecords.ToArray();
			}

			data.KillCount++;
			data.WinScore += _WinScore;
		}
	}
}
