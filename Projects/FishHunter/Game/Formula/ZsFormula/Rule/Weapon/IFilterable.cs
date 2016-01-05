using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Weapon
{
	public interface IFilterable
	{
		RequsetFishData[] Filter(RequsetFishData[] fish_datas);
	}
}