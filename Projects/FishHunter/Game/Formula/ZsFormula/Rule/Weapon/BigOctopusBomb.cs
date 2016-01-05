using System.Linq;

using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Weapon
{
	public class BigOctopusBomb : IFilterable
	{
		// : Provider<BigOctopusBomb>
		RequsetFishData[] IFilterable.Filter(RequsetFishData[] fish_datas)
		{
			var t = from fish in fish_datas
					where fish.FishType < FISH_TYPE.SPECIAL_FREEZE_BOMB
					select fish;

			return t.ToArray();
		}
	}
}
