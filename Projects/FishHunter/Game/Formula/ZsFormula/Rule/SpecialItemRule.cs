using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     取得道具
	/// </summary>
	public class SpecialItemRule
	{
		private readonly StageDataVisitor _StageVisitor;

		public SpecialItemRule(StageDataVisitor stage_visitor)
		{
			_StageVisitor = stage_visitor;
		}

		public void Run()
		{
			// 武器出現次數+1
//			_StageVisitor.PlayerRecord.NowWeaponPower.WinFrequency++;
//
//			// 武器出現次數+1
//			var stageWeaponData =
//				_StageVisitor.FocusFishFarmData.RecordData.SpecialWeaponDatas.Find(x => x.SpId == _StageVisitor.PlayerRecord.NowWeaponPower.SpId);
//			stageWeaponData.WinFrequency++;
		}
	}
}
