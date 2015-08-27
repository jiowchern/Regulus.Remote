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
		[EnumDescription("小紅魚 熱帶小魚")]
		TROPICAL_FISH = 0,

		/// <summary>
		///     條紋魚(神仙魚)
		/// </summary>
		[EnumDescription("條紋魚(神仙魚)")]
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
		///     銀鯨
		/// </summary>
		[EnumDescription("銀鯨")]
		WHALE_SLIVER,

		/// <summary>
		///     彩鯨
		/// </summary>
		[EnumDescription("彩鯨")]
		WHALE_COLOR,

		/// <summary>
		///     大三元 大四喜
		/// </summary>
		[EnumDescription("大三元、大四喜")]
		SPECIAL_RING,

		/// <summary>
		///     大魚吃小魚
		/// </summary>
		[EnumDescription("大魚吃小魚")]
		SPECIAL_EAT_FISH,

        /// <summary>
        ///     鳯凰 200~600倍, X1~X10
        /// </summary>
        [EnumDescription("鳯凰、大魚吃小魚600倍")]
		SPECIAL_EAT_FISH_CRAZY,

		/// <summary>
		///     特殊魚往下開始
		/// </summary>

		/// <summary>
		///     冰凍碼錶
		/// </summary>
		[EnumDescription("冰凍碼錶")]
        SPECIAL_FREEZE_BOMB,

        /// <summary>
        ///     全屏炸彈
        /// </summary>
        [EnumDescription("全屏炸彈")]
		SPECIAL_SCREEN_BOMB,

		/// <summary>
		///     皮卡丘
		/// </summary>
		[EnumDescription("皮卡丘")]
		SPECIAL_THUNDER_BOMB, 

		/// <summary>
		///     貪食蛇
		/// </summary>
		[EnumDescription("貪食蛇")]
		SPECIAL_FIRE_BOMB, 

		/// <summary>
		///     河豚
		/// </summary>
		[EnumDescription("河豚")]
		SPECIAL_DAMAGE_BALL, 

		/// <summary>
		///     小章魚
		/// </summary>
		[EnumDescription("小章魚")]
		SPECIAL_OCTOPUS_BOMB, 

		/// <summary>
		///     大章魚
		/// </summary>
		[EnumDescription("大章魚")]
		SPECIAL_BIG_OCTOPUS_BOMB, 
	}
}
