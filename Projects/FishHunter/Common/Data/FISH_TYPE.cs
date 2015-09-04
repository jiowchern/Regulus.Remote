using Regulus.Utility;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	///     所有魚的定義
	/// </summary>
	public enum FISH_TYPE
	{
		/// <summary>
		///     小紅魚(熱帶小魚)
		/// </summary>
		[EnumDescription("0小紅魚 熱帶小魚")]
		TROPICAL_FISH = 0,

		/// <summary>
		///     條紋魚(神仙魚)
		/// </summary>
		[EnumDescription("1條紋魚(神仙魚)")]
		ANGEL_FISH = 1, 

		/// <summary>
		///     海星
		/// </summary>
		[EnumDescription("2海星")]
		START_FISH = 2, 

		/// <summary>
		///     海馬
		/// </summary>
		[EnumDescription("3海馬")]
		HIPPOCAMPUS = 3, 

		/// <summary>
		///     龍蝦
		/// </summary>
		[EnumDescription("4龍蝦")]
		LOBSTER = 4, 

		/// <summary>
		///     水母
		/// </summary>
		[EnumDescription("5水母")]
		JELLYFISH = 5, 

		/// <summary>
		///     飛魚
		/// </summary>
		[EnumDescription("6飛魚")]
		FLYINGFISH = 6, 

		/// <summary>
		///     海龜
		/// </summary>
		[EnumDescription("7烏龜")]
		TORTOISE = 7, 

		/// <summary>
		///     魟魚
		/// </summary>
		[EnumDescription("8魟魚")]
		STINGRAY = 8, 

		/// <summary>
		///     鸚鵡螺
		/// </summary>
		[EnumDescription("9鸚鵡螺")]
		NAUTILUS = 9, 

		/// <summary>
		///     魷魚
		/// </summary>
		[EnumDescription("10魷魚")]
		SQUID = 10, 

		/// <summary>
		///     獅子魚
		/// </summary>
		[EnumDescription("11獅子魚")]
		LIONFISH = 11, 

		/// <summary>
		///     曼波
		/// </summary>
		[EnumDescription("12曼波魚")]
		MOLA = 12, 

		/// <summary>
		///     燈籠魚
		/// </summary>
		[EnumDescription("13燈籠")]
		LANTERN = 13, 

		/// <summary>
		///     海精靈
		/// </summary>
		[EnumDescription("14海精靈")]
		SEA_ELVES = 14, 

		/// <summary>
		///     鱘龍魚
		/// </summary>
		[EnumDescription("15鱘龍魚")]
		STURGEON_AROWANA = 15, 

		/// <summary>
		///     槌頭鯊
		/// </summary>
		[EnumDescription("16槌頭鯊")]
		HAMMERHEAD_SHARK = 16, 

		/// <summary>
		///     白鯨
		/// </summary>
		[EnumDescription("17小白鯨")]
		BELUGA_WHALES = 17, 

		/// <summary>
		///     藍鯨
		/// </summary>
		[EnumDescription("18藍鯨")]
		BLUE_WHALE = 18, 

		/// <summary>
		///     紅鯨
		/// </summary>
		[EnumDescription("19紅鯨")]
		RED_WHALE = 19, 

		/// <summary>
		///     金鯨
		/// </summary>
		[EnumDescription("20金鯨")]
		GOLDEN_WHALE = 20, 

		/// <summary>
		///     銀鯨
		/// </summary>
		[EnumDescription("21銀鯨")]
		WHALE_SLIVER = 21,

		/// <summary>
		///     彩鯨
		/// </summary>
		[EnumDescription("22彩鯨")]
		WHALE_COLOR = 22,

		/// <summary>
		///     大三元 大四喜
		/// </summary>
		[EnumDescription("23大三元、大四喜")]
		SPECIAL_RING = 23,

		/// <summary>
		///     大魚吃小魚
		/// </summary>
		[EnumDescription("24大魚吃小魚")]
		SPECIAL_EAT_FISH = 24,

		/// <summary>
		///     鳯凰 200~600倍, X1~X10
		/// </summary>
		[EnumDescription("25鳯凰、大魚吃小魚600倍")]
		SPECIAL_EAT_FISH_CRAZY = 25,

		/// <summary>
		///     特殊魚往下開始
		/// </summary>

		/// <summary>
		///     冰凍碼錶
		/// </summary>
		[EnumDescription("26冰凍碼錶")]
		SPECIAL_FREEZE_BOMB = 26,

		/// <summary>
		///     全屏炸彈
		/// </summary>
		[EnumDescription("27全屏炸彈")]
		SPECIAL_SCREEN_BOMB = 27,

		/// <summary>
		///     皮卡丘
		/// </summary>
		[EnumDescription("28皮卡丘")]
		SPECIAL_THUNDER_BOMB = 28, 

		/// <summary>
		///     貪食蛇
		/// </summary>
		[EnumDescription("29貪食蛇")]
		SPECIAL_FIRE_BOMB = 29, 

		/// <summary>
		///     河豚
		/// </summary>
		[EnumDescription("30河豚")]
		SPECIAL_DAMAGE_BALL = 30, 

		/// <summary>
		///     小章魚
		/// </summary>
		[EnumDescription("31小章魚")]
		SPECIAL_OCTOPUS_BOMB = 31, 

		/// <summary>
		///     大章魚
		/// </summary>
		[EnumDescription("32大章魚")]
		SPECIAL_BIG_OCTOPUS_BOMB = 32, 
	}
}
