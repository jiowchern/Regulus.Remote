using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     取得道具
	/// </summary>
	public class SpecialItemRule
	{
		private readonly FarmDataVisitor _FarmVisitor;

		public SpecialItemRule(FarmDataVisitor farm_visitor)
		{
			_FarmVisitor = farm_visitor;
		}

		public void Run()
		{
			// 武器出現次數+1
//			_FarmVisitor.PlayerRecord.NowWeaponPower.WinFrequency++;
//
//			// 武器出現次數+1
//			var stageWeaponData =
//				_FarmVisitor.FocusFishFarmData.RecordData.SpecialWeaponDatas.Find(x => x.SpId == _FarmVisitor.PlayerRecord.NowWeaponPower.SpId);
//			stageWeaponData.WinFrequency++;
		}
	}
}
