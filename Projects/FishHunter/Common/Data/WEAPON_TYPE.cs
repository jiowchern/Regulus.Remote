using Regulus.Utility;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	///     伤害类型,公式判断用
	/// </summary>
	internal enum DAMAGE_TYPE
	{
		NORMAL, 

		FULL_SCREEN_BOMB_1, // 死一半

		FULL_SCREEN_BOMB_2, 

		ICE_
	}

	/// <summary>
	///     武器類型
	/// </summary>
	public enum WEAPON_TYPE
	{
		/// <summary>
		///     0 無效
		/// </summary>
		INVALID, 

		/// <summary>
		///     1一般武器
		/// </summary>
		NORMAL = 1, // 正常子彈

		/// <summary>
		///     2超級炮
		/// </summary>
		SUPER_BOMB = 2, 

		/// <summary>
		///     3電網
		/// </summary>
		ELECTRIC_NET = 3, 

		/// <summary>
		///     4免費炮
		/// </summary>
		FREE_POWER = 4, 

		/// <summary>
		///     101全屏炸彈
		/// </summary>
		SCREEN_BOMB = 101, 

		/// <summary>
		///     皮卡丘 電鰻
		/// </summary>
		[EnumDescription("皮卡丘")]
		THUNDER_BOMB = 102, 

		/// <summary>
		///     貪食蛇
		/// </summary>
		[EnumDescription("貪食蛇")]
		FIRE_BOMB = 103, 

		/// <summary>
		///     河豚 鐵球
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
		BIG_OCTOPUS_BOMB = 106
	};
}
