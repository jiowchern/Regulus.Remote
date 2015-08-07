using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.ZsFormula.Data;

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	/// <summary>
	///     取得道具
	/// </summary>
	public class SpecialItemRule
	{
		private readonly PlayerRecord _PlayerRecord;

		private readonly FishStageVisitor _StageVisitor;

		public SpecialItemRule(FishStageVisitor stage_visitor, PlayerRecord player_record)
		{
			_StageVisitor = stage_visitor;
			_PlayerRecord = player_record;
		}

		public void Run()
		{
			if (_PlayerRecord.NowSpecialWeaponData.HaveWeapon)
			{
				return;
			}

			// 武器出現次數+1
			_PlayerRecord.NowSpecialWeaponData.WinFrequency++;

			// 武器出現次數+1
			var stageWeaponData =
				_StageVisitor.NowData.RecordData.SpecialWeaponDatas.Find(x => x.SpId == _PlayerRecord.NowSpecialWeaponData.SpId);
			stageWeaponData.WinFrequency++;
		}
	}
}