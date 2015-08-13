using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	public class WeaponTypeRule
	{
		private readonly StageDataVisitor _StageVisitor;

		private readonly RequestWeaponData _WeaponData;

		private readonly int _Win;

		public WeaponTypeRule(
			StageDataVisitor stage_visitor, 
			RequestWeaponData weapon_data, 
			int win)
		{
			_StageVisitor = stage_visitor;
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
//				_StageVisitor.PlayerRecord.FindStageRecord(_StageVisitor.FocusStageData.StageId)
//				             .SpecialWeaponDatas.Find(x => x.WeaponType == _WeaponData.WeaponType);
//			data.WinScore += _Win;
//
//			// stageªº
//			var stageWeaponData =
//				_StageVisitor.FocusStageData.RecordData.SpecialWeaponDatas.Find(x => x.WeaponType == _WeaponData.WeaponType);
//			stageWeaponData.WinScore += _Win;
		}
	}
}
