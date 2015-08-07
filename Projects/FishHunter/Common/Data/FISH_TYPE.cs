
using Regulus.Utility;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	/// 所有魚的定義
	/// </summary>
	public enum FISH_TYPE
	{
		/// <summary>
		///	一般魚開始
		/// </summary>
		[EnumDescription("一般魚開始")]
		ORDINARY_FISH_BEGIN = 0,
		
		/// <summary>
		///		小紅魚(熱帶小魚)
		/// </summary>
		[EnumDescription("熱帶小魚")]
		TROPICAL_FISH = FISH_TYPE.ORDINARY_FISH_BEGIN, 

		/// <summary>
		///     條紋魚(神仙魚)
		/// </summary>
		[EnumDescription("熱帶小魚")]
		ANGEL_FISH, 

		/// <summary>
		///     海星
		/// </summary>
		[EnumDescription("海星")]
		START_FISH, 

		/// <summary>
		///     海馬
		/// </summary>
		[EnumDescription("海馬")]
		HIPPOCAMPUS, 

		/// <summary>
		///     龍蝦
		/// </summary>
		[EnumDescription("龍蝦")]
		LOBSTER, 

		/// <summary>
		///     水母
		/// </summary>
		[EnumDescription("水母")]
		JELLYFISH, 

		/// <summary>
		///     飛魚
		/// </summary>
		[EnumDescription("飛魚")]
		FLYINGFISH, 

		/// <summary>
		///     海龜
		/// </summary>
		[EnumDescription("烏龜")]
		TORTOISE, 

		/// <summary>
		///     魟魚
		/// </summary>
		[EnumDescription("魟魚")]
		STINGRAY, 

		/// <summary>
		///     鸚鵡螺
		/// </summary>
		[EnumDescription("鸚鵡螺")]
		NAUTILUS, 

		/// <summary>
		///     魷魚
		/// </summary>
		[EnumDescription("魷魚")]
		SQUID, 

		/// <summary>
		///     獅子魚
		/// </summary>
		[EnumDescription("獅子魚")]
		LIONFISH, 

		/// <summary>
		///     曼波
		/// </summary>
		[EnumDescription("曼波魚")]
		MOLA, 

		/// <summary>
		///     燈籠魚
		/// </summary>
		[EnumDescription("燈籠")]
		LANTERN, 

		/// <summary>
		///     海精靈
		/// </summary>
		[EnumDescription("海精靈")]
		SEA_ELVES, 

		/// <summary>
		///     鱘龍魚
		/// </summary>
		[EnumDescription("鱘龍魚")]
		STURGEON_AROWANA, 

		/// <summary>
		///     槌頭鯊
		/// </summary>
		[EnumDescription("槌頭鯊")]
		HAMMERHEAD_SHARK, 

		/// <summary>
		///     白鯨
		/// </summary>
		[EnumDescription("小白鯨")]
		BELUGA_WHALES, 

		/// <summary>
		///     藍鯨
		/// </summary>
		[EnumDescription("藍鯨")]
		BLUE_WHALE, 

		/// <summary>
		///     紅鯨
		/// </summary>
		[EnumDescription("紅鯨")]
		RED_WHALE, 

		/// <summary>
		///     金鯨
		/// </summary>
		[EnumDescription("金鯨")]
		GOLDEN_WHALE,

		/// <summary>
		/// 一般魚結束
		/// </summary>
		[EnumDescription("一般魚結束")]
		ORDINARY_FISH_END = FISH_TYPE.GOLDEN_WHALE,

		/// <summary>
		///     特殊魚開始
		/// </summary>
		[EnumDescription("特殊魚開始")]
		SPECIAL_FISH_BEGIN = FISH_TYPE.WHALE_SLIVER, 

		/// <summary>
		///     銀鯨
		/// </summary>
		[EnumDescription("銀鯨")]
		WHALE_SLIVER = 94, 

		/// <summary>
		///     彩鯨
		/// </summary>
		[EnumDescription("彩鯨")]
		WHALE_COLOR = 95, 

		/// <summary>
		///     魚王
		/// </summary>
		[EnumDescription("魚王")]
		SAME_BOMB = 96, 

		/// <summary>
		///     大三元 大四喜
		/// </summary>
		[EnumDescription("大三元、大四喜")]
		RING = 97, 

		/// <summary>
		///     冰凍碼錶
		/// </summary>
		[EnumDescription("冰凍碼錶")]
		FREEZE_BOMB = 98, 

		/// <summary>
		///     大魚吃小魚
		/// </summary>
		[EnumDescription("大魚吃小魚")]
		EAT_FISH = 99, 

		/// <summary>
		///     鳯凰 200~600倍, X1~X10
		/// </summary>
		[EnumDescription("鳯凰")]
		PHOENIX = 100, 

		/// <summary>
		///     全屏炸彈
		/// </summary>
		[EnumDescription("全屏炸彈")]
		SCREEN_BOMB = 101, 

		/// <summary>
		///     皮卡丘
		/// </summary>
		[EnumDescription("皮卡丘")]
		THUNDER_BOMB = 102, 

		/// <summary>
		///     貪食蛇
		/// </summary>
		[EnumDescription("貪食蛇")]
		FIRE_BOMB = 103, 

		/// <summary>
		///     河豚
		/// </summary>
		[EnumDescription("河豚")]
		DAMAGE_BALL = 104, 

		/// <summary>
		///     小章魚
		/// </summary>
		[EnumDescription("小章魚")]
		OCTOPUS_BOMB = 105, 

		/// <summary>
		///     大章魚
		/// </summary>
		[EnumDescription("大章魚")]
		BIG_OCTOPUS_BOMB = 106, 

		/// <summary>
		///     特殊魚結束
		/// </summary>
		[EnumDescription("特殊魚結束")]
		SPECAIL_FISH_END = FISH_TYPE.BIG_OCTOPUS_BOMB
	}
}