using System.Linq;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Save
{
	/// <summary>
	///     記錄擊殺魚的資料
	/// </summary>
	public class SaveDeathFishHistory
	{
		private readonly DataVisitor _DataVisitor;

		private readonly RequsetFishData _Fish;

		private readonly RequestWeaponData _WeaponData;

		public SaveDeathFishHistory(DataVisitor data_visitor, RequsetFishData fish, RequestWeaponData weapon_data_data)
		{
			_Fish = fish;
			_WeaponData = weapon_data_data;
			_DataVisitor = data_visitor;
		}

		public void Run()
		{
			_SavePlayerWeapon();
			_SaveFarmWeapon();
		}

		private void _SavePlayerWeapon()
		{
			var hitRecords = _DataVisitor.PlayerRecord.FindFarmRecord(_DataVisitor.Farm.FarmId);

			_Save(hitRecords);
		}

		private void _SaveFarmWeapon()
		{
			var hitRecords = _DataVisitor.Farm.Record;

			_Save(hitRecords);
		}

		private void _Save(FarmRecord farm_record)
		{
			var list = farm_record.WeaponHitRecords.ToList();

			var record = list.FirstOrDefault(x => x.WeaponType == _WeaponData.WeaponType);

			if(record == null)
			{
				record = new WeaponHitRecord(_WeaponData.WeaponType);

				list.Add(record);
			}

			_SetKills(record);

			record.TotalOdds += _Fish.GetRealOdds();

			record.WinScore += _Fish.GetRealOdds() * _WeaponData.GetTotalBet();

			farm_record.WeaponHitRecords = list.ToArray();
		}

		private void _SetKills(WeaponHitRecord record)
		{
			var list = record.FishKills.ToList();

			var dieFish = list.FirstOrDefault(x => x.FishType == _Fish.FishType);

			if(dieFish == null)
			{
				dieFish = new FishKillRecord(_Fish.FishType);
				list.Add(dieFish);
			}

			dieFish.KillCount++;

			record.FishKills = list.ToArray();
		}
	}
}
