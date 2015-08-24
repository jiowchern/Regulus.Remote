using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	public class WeaponTypeRule
	{
		private readonly DataVisitor _Visitor;

		private readonly RequestWeaponData _WeaponData;

		private readonly int _Win;

		public WeaponTypeRule(
			DataVisitor visitor, 
			RequestWeaponData weapon_data, 
			int win)
		{
			_Visitor = visitor;
			_WeaponData = weapon_data;
			_Win = win;
		}

		public void Run()
		{
			//return;
//			if(_Win == 0)
//			{
//				return;
//			}
//
//			if(_WeaponData.WeaponType != WEAPON_TYPE.FREE_POWER)
//			{
//				return;
//			}
//
//			// ª±®aªº
//			var data =
//				_Visitor.PlayerRecord.FindFarmRecord(_Visitor.Farm.FarmId)
//				             .SpecialWeaponDatas.Find(x => x.WeaponType == _WeaponData.WeaponType);
//			data.WinScore += _Win;
//
//			// stageªº
//			var stageWeaponData =
//				_Visitor.Farm.Record.SpecialWeaponDatas.Find(x => x.WeaponType == _WeaponData.WeaponType);
//			stageWeaponData.WinScore += _Win;
		}
	}
}
