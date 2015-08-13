using Regulus.Remoting;

namespace VGame.Project.FishHunter.Common.GPI
{
	public interface ILevelSelector
	{
		Value<int[]> QueryStages();

		Value<bool> Select(int level);
	}
}
