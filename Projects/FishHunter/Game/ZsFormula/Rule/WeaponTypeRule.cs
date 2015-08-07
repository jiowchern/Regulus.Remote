using System;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.ZsFormula.Data;

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	public class WeaponTypeRule
	{
		private readonly PlayerRecord _PlayerRecord;

		private readonly FishStageVisitor _StageVisitor;

		private readonly RequestWeaponData _WeaponData;

		private readonly int _Win;

		public WeaponTypeRule(FishStageVisitor stage_visitor, RequestWeaponData weapon_data, PlayerRecord player_record, int win)
		{
			_StageVisitor = stage_visitor;
			_WeaponData = weapon_data;
			_PlayerRecord = player_record;
			_Win = win;
		}

		public void Run()
		{
			if (_Win == 0)
			{
				return;
			}

			if (_WeaponData.WeaponType != WEAPON_TYPE.FREE_POWER)
			{
				return;
			}

			// ª±®aªº
			var data =
				_PlayerRecord.FindStageRecord(_StageVisitor.NowData.StageId)
					.SpecialWeaponDatas.Find(x => x.WeaponType == _WeaponData.WeaponType);
			data.WinScore += _Win;

			// stageªº
			var stageWeaponData =
				_StageVisitor.NowData.RecordData.SpecialWeaponDatas.Find(x => x.WeaponType == _WeaponData.WeaponType);
			stageWeaponData.WinScore += _Win;
		}
	}
}