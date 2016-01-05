using System;
using System.Collections.Generic;
using System.Linq;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule.FloatingOdds;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Weapon
{
	/// <summary>
	///     皮卡丘
	/// </summary>
	public class ThunderBomb : IFilterable
	{
		RequsetFishData[] IFilterable.Filter(RequsetFishData[] fish_datas)
		{
			// 只电10倍以上的鱼
			var filter1 = from fish in fish_datas
						where fish.FishOdds >= 10
						select fish;

			// 最多电15只鱼
			var filter2 = new List<RequsetFishData>();

			foreach(var t in filter1.TakeWhile(t => filter2.Count <= 15))
			{
				filter2.Add(t);
			}

			return filter2.ToArray();
		}
	}
}
