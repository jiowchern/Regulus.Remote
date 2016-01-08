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
			var fishHitRecords = _Visitor.PlayerRecord.FindFarmRecord(_Visitor.Farm.FarmId)
										.FishHits;

			_SaveTreasureHistory(fishHitRecords, fish);
		}

		private void _RecordTreasureForFarm(RequsetFishData fish)
		{
			var fishHitRecords = _Visitor.Farm.Record.FishHits;

			_SaveTreasureHistory(fishHitRecords, fish);
		}

		private void _SaveTreasureHistory(IEnumerable<FishHitRecord> fish_hit_records, RequsetFishData fish)
		{
			var record = fish_hit_records.FirstOrDefault(x => x.FishType == fish.FishType);

			if(record == null)
			{
				return;
			}

			var list = record.TreasureRecords.ToList();

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

			record.TreasureRecords = list.ToArray();
		}
	}
}
