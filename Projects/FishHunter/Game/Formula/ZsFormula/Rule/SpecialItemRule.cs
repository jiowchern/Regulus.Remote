using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     取得道具
	/// </summary>
	public class SpecialItemRule
	{
		private readonly DataVisitor _Visitor;

		public SpecialItemRule(DataVisitor visitor)
		{
			_Visitor = visitor;
		}

		public void Run()
		{
			// 武器出現次數+1
//			_Visitor.PlayerRecord.NowWeaponPower.WinFrequency++;
//
//			// 武器出現次數+1
//			var stageWeaponData =
//				_Visitor.Farm.Record.SpecialWeaponDatas.Find(x => x.SpId == _Visitor.PlayerRecord.NowWeaponPower.SpId);
//			stageWeaponData.WinFrequency++;
		}
	}
}
