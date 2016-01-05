using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Save;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation
{
	public class Record : IPipelineElement
	{
		private readonly HitRequest _Request;

		private readonly DataVisitor _Visitor;

		public Record(DataVisitor visitor, HitRequest request)
		{
			_Visitor = visitor;
			_Request = request;
		}

		bool IPipelineElement.IsComplete
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		void IPipelineElement.Connect(IPipelineElement next)
		{
			throw new NotImplementedException();
		}

		void IPipelineElement.Process()
		{
			_RecordScore();
			_RecordToFocusBlock();
			_RecordToFarm();
			_RecordToPlayer();

			_RecordTreasureForPlayer();
			_RecordTreasureForFarm();
		}

		private void _RecordScore()
		{
			foreach(var fishData in _Request.FishDatas)
			{
				var bet = _Request.WeaponData.GetTotalBet();
				var winScore = fishData.FishOdds * bet * fishData.OddsValue;
				new SaveDeathFishHistory(_Visitor, fishData, winScore).Run();
				new SaveScoreHistory(_Visitor, winScore).Run();
			}
		}

		private void _RecordToFocusBlock()
		{
			var normal = _Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName, FarmDataRoot.BufferNode.BUFFER_NAME.NORMAL);

			normal.Block.FireCount += 1;
			normal.Block.TotalSpending += _Request.WeaponData.GetTotalBet();
		}

		private void _RecordToPlayer()
		{
			_Visitor.PlayerRecord.FindFarmRecord(_Visitor.Farm.FarmId)
					.FireCount += 1;

			_Visitor.PlayerRecord.FindFarmRecord(_Visitor.Farm.FarmId)
					.TotalSpending += _Request.WeaponData.GetTotalBet();
		}

		private void _RecordToFarm()
		{
			_Visitor.Farm.Record.FireCount += 1;
			_Visitor.Farm.Record.TotalSpending += _Request.WeaponData.GetTotalBet();
		}

		private void _RecordTreasureForPlayer()
		{
			var fishHitRecords = _Visitor.PlayerRecord.FindFarmRecord(_Visitor.Farm.FarmId)
										.FishHits;

			foreach(var fish in _Request.FishDatas.Where(fish => Singleton<DeathMonitor>.Instance.IsDied(fish.FishId)))
			{
				_SaveTreasureHistory(fishHitRecords, fish);
			}
		}

		private void _RecordTreasureForFarm()
		{
			var fishHitRecords = _Visitor.Farm.Record.FishHits;

			foreach(var fish in _Request.FishDatas.Where(fish => Singleton<DeathMonitor>.Instance.IsDied(fish.FishId)))
			{
				_SaveTreasureHistory(fishHitRecords, fish);
			}
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
