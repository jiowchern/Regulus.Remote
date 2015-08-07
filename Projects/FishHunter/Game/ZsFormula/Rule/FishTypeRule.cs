
using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.ZsFormula.Data;

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	/// <summary>
	///     記下特殊魚拿到幾次
	/// </summary>
	public class FishTypeRule
	{
		private readonly RequsetFishData _Fish;

		private readonly PlayerRecord _PlayerRecord;

		private readonly FishStageVisitor _StageVisitor;

		public FishTypeRule(FishStageVisitor stage_visitor, RequsetFishData fish, PlayerRecord player_record)
		{
			_Fish = fish;
			_PlayerRecord = player_record;
			_StageVisitor = stage_visitor;
		}

		public void Run()
		{
			if(_Fish.FishType < FISH_TYPE.SPECIAL_FISH_BEGIN)
			{
				return;
			}

			// 沒用過的就可以
			// 存玩家的
			var data =
				_PlayerRecord.FindStageRecord(_StageVisitor.NowData.StageId)
				           .SpecialWeaponDatas.Find(x => (int)x.WeaponType == (int)_Fish.FishType);

			// data.SpId = _Fish.FishType;
			data.WinFrequency++;

			// 存stage的
			var stageData =
				_StageVisitor.NowData.RecordData.SpecialWeaponDatas.Find(x => (int)x.WeaponType == (int)_Fish.FishType);

			// stageData.SpId = (int)_Fish.FishType;
			stageData.WinFrequency++;
		}
	}
}
