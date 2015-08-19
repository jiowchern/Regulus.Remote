using Regulus.Utility;

namespace VGame.Project.FishHunter.Common.Data
{
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
		SCREEN_BOMB, 

		/// <summary>
		///     皮卡丘 電鰻
		/// </summary>
		[EnumDescription("皮卡丘")]
		THUNDER_BOMB, 

		/// <summary>
		///     貪食蛇
		/// </summary>
		[EnumDescription("貪食蛇")]
		FIRE_BOMB, 

		/// <summary>
		///     河豚 鐵球
		/// </summary>
		[EnumDescription("河豚")]
		DAMAGE_BALL, 

		/// <summary>
		///     小章魚
		/// </summary>
		[EnumDescription("小章魚")]
		OCTOPUS_BOMB, 

		/// <summary>
		///     大章魚
		/// </summary>
		[EnumDescription("大章魚")]
		BIG_OCTOPUS_BOMB
	};
}
