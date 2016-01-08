using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule.Weapon;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Save
{
//	public sealed class DeathMonitor : Singleton<DeathMonitor>
//	{
//		private const int _Sizes = 100;
//
//		private readonly Queue<int> _DeathFishs;
//
//		public List<RequsetFishData> DeadFishs { get; private set; }
//
//		public DeathMonitor()
//		{
//			_DeathFishs = new Queue<int>(_Sizes);
//			DeadFishs = new List<RequsetFishData>();
//        }
//
//		public void FilterAllDead(RequsetFishData[] fish_datas)
//		{
//			DeadFishs = fish_datas.TakeWhile(t => IsDied(t.FishId))
//								.ToList();
//		}
//
//		public RequsetFishData[] GetAllAliving(RequsetFishData[] fish_datas)
//		{
//			var filter = fish_datas.SkipWhile(t => IsDied(t.FishId))
//									.ToList();
//
//			return filter.ToArray();
//		}
//
//		public void Run(RequsetFishData fish)
//		{
//			if(IsDied(fish.FishId))
//			{
//				return;
//			}
//
//			if(_DeathFishs.Count >= _Sizes)
//			{
//				_DeathFishs.Dequeue();
//			}
//
//			fish.IsDead = true;
//			_DeathFishs.Enqueue(fish.FishId);
//		}
//
//		public bool IsDied(int fish_id)
//		{
//			return _DeathFishs.Contains(fish_id);
//		}
//	}
}
