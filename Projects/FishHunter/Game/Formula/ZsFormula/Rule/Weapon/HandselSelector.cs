using System.Linq;

using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Weapon
{
	/// <summary>
	///     特殊鱼送一只必死
	///     铁球、海绵、皮卡丘、贪食蛇、小章鱼、隐藏全屏、隐藏电网 这类会爆炸的
	///     判定第一只小于30倍的鱼必死，降低玩家获得 0分的机会
	///		只能武器第一次判定送禮
	/// </summary>
	public class HandselSelector
	{
		private const int _GiftOdds = 30;

		public static void Select(HitRequest hit_request)
		{
			if(!hit_request.IsFirstTrigger)
			{
				return;
			}

			var giftFish = hit_request.FishDatas.FirstOrDefault(x => x.FishOdds <= _GiftOdds);

			if(giftFish != null)
			{
				giftFish.IsGift = true;
			}
		}
	}
}
