using System.Collections.Generic;
using System.Linq;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Save
{
	public class SaveTreasure
	{
		private readonly RequsetFishData _FishData;

		private readonly DataVisitor _Visitor;

		public SaveTreasure(DataVisitor visitor, RequsetFishData fish_data)
		{
			_Visitor = visitor;
			_FishData = fish_data;
		}

		public void Run()
		{
			_RecordTreasureForPlayer(_FishData);
			_RecordTreasureForFarm(_FishData);
		}

		private void _RecordTreasureForPlayer(RequsetFishData fish)
		{
			var fishHitRecords = _Visitor.PlayerRecord.FindFarmRecord(_Visitor.Farm.FarmId);

			_SaveTreasureHistory(fishHitRecords);
		}

		private void _RecordTreasureForFarm(RequsetFishData fish)
		{
			var fishHitRecords = _Visitor.Farm.Record;

			_SaveTreasureHistory(fishHitRecords);
		}

		private void _SaveTreasureHistory(FarmRecord fish_hit_records)
		{
			var list = fish_hit_records.RandomTreasures.ToList();

			foreach(var treasure in _Visitor.GetAllTreasures())
			{
				var d = list.FirstOrDefault(x => x.WeaponType == treasure);
				if(d == null)
				{
					d = new TreasureRecord(treasure);
					list.Add(d);
				}

				d.Count++;
			}

			fish_hit_records.RandomTreasures = list.ToArray();
		}
	}
}
