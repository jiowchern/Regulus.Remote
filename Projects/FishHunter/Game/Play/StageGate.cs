using System.Collections.Generic;
using System.Linq;


using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Play
{
	internal class StageGate
	{
		private readonly StageLock[] _Locks;

		public StageGate(StageLock[] locks)
		{
			_Locks = locks;
		}

		internal IEnumerable<int> FindUnlockStage(IEnumerable<int> passs, int kill_count)
		{
			var unlocks = new List<int>();
			foreach(var l in _Locks)
			{
				var total = l.Requires.Length;
				if(l.Requires.Intersect(passs).Count() == total)
				{
					if(l.KillCount <= kill_count)
					{
						unlocks.Add(l.Stage);
					}
				}
			}

			return unlocks.Except(passs);
		}
	}
}
