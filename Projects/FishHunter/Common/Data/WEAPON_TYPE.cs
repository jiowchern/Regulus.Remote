using ProtoBuf;


using Regulus.Utility;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	///     武器類型
	/// </summary>
	[ProtoContract]
	public enum WEAPON_TYPE
	{
		/// <summary>
		///     0 無效
		/// </summary>
		[EnumDescription("0無效")]
		INVALID = 0,

		/// <summary>
		///     1一般武器
		/// </summary>
		[EnumDescription("1一般子彈")]
		NORMAL = 1, // 正常子彈

		/// <summary>
		///     2超級炮
		/// </summary>
		[EnumDescription("2超級炮")]
		SUPER_BOMB = 2,

		/// <summary>
		///     3電網
		/// </summary>
		[EnumDescription("3電網")]
		ELECTRIC_NET = 3,

		/// <summary>
		///     4免費炮
		/// </summary>
		[EnumDescription("4免費炮")]
		FREE_POWER = 4,

		/// <summary>
		///     101全屏炸彈
		/// </summary>
		[EnumDescription("5全屏炸彈")]
		SCREEN_BOMB = 5, 

		/// <summary>
		///     皮卡丘 電鰻
		/// </summary>
		[EnumDescription("6皮卡丘、電鰻")]
		THUNDER_BOMB = 6, 

		/// <summary>
		///     貪食蛇
		/// </summary>
		[EnumDescription("7貪食蛇")]
		FIRE_BOMB = 7, 

		/// <summary>
		///     河豚 鐵球
		/// </summary>
		[EnumDescription("8河豚")]
		DAMAGE_BALL = 8, 

		/// <summary>
		///     小章魚
		/// </summary>
		[EnumDescription("9小章魚")]
		OCTOPUS_BOMB = 9, 

		/// <summary>
		///     大章魚
		/// </summary>
		[EnumDescription("10大章魚")]
		BIG_OCTOPUS_BOMB = 10,

		/// <summary>
		/// 魚王
		/// </summary>
		[EnumDescription("11魚王")]
		KING = 11,

		/// <summary>
		/// 冰凍碼錶(炸彈)
		/// </summary>
		[EnumDescription("12冰凍碼錶")]
		FREEZE_BOMB = 12,
	};
}
