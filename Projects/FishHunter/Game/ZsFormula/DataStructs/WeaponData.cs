// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeaponData.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the WeaponOddsRuler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Collections.Generic;
using System.Linq;

using Regulus.Utility;

namespace VGame.Project.FishHunter.ZsFormula.DataStructs
{
	/*
	 武器的 _Attack.WepType
	1为一般子弹，2~4为特殊武器（与返回值相同），
	 * 100以后为特殊鱼变倍鱼的处理方式
	吃小鱼绿安康（100~200）、吃小鱼红安康（60~600）、变倍银鲸（100~300/50）、变倍七彩鲸（300~600/50）
	 * ULONG const ulChanceSpecOdds[3] = { 250 , 150 , 120 } ; // 炸弹、电网、免费炮
*/
	public class WeaponDataTable
	{
		public List<Data> WeaponDatas;

		public WeaponDataTable()
		{
			WeaponDatas = new List<Data>
			{
				new Data(type: Data.WEAPON_TYPE.NORMAL_1, power: Data.WEAPON_POWER.BOMB_2, odds: 100, bet: 100), 

				new Data(type: Data.WEAPON_TYPE.BOMB_2, power: Data.WEAPON_POWER.BOMB_2, odds: 100, bet: 100), 

				new Data(Data.WEAPON_TYPE.ELECTRIC_GRID_3, Data.WEAPON_POWER.ELECTRIC_GRID_3, 100, 100), 
				
				new Data(Data.WEAPON_TYPE.TYPE_101, Data.WEAPON_POWER.FREE_1, 100, 100), 
				
				new Data(Data.WEAPON_TYPE.TYPE_102, Data.WEAPON_POWER.FREE_1, 100, 100)
			};
		}

		public class Data
		{
			public enum WEAPON_POWER
			{
				FREE_1 = 120, // 免费炮 //倍率 打了一隻小於120倍的魚，理論上一定死

				BOMB_2 = 250, // 炸弹

				ELECTRIC_GRID_3 = 150 // 电网
			}

			public enum WEAPON_TYPE
			{
				NORMAL_1 = 1, // 正常子彈

				BOMB_2 = 2, 

				ELECTRIC_GRID_3 = 3,

				FREE_4 = 4, // 免費炮

				// 特殊魚的編號跟特武編號一樣
				TYPE_101 = 101, // 海綿寶寶

				TYPE_102 = 102, // 電鰻

				TYPE_103 = 103, // 貪食蛇

				TYPE_104 = 104, // 鐵球

				TYPE_105 = 105, // 小章魚

				TYPE_106 = 106, // 大章魚

				TYPE_CONUT
			}

			public WEAPON_POWER Power { get; set; }

			public WEAPON_TYPE WeaponType { get; set; }

			public int WeaponOdds { get; set; }

			public int WeaponBet { get; set; }


			public Data(WEAPON_TYPE type, WEAPON_POWER power, int odds, int bet)
			{
				WeaponType = type;
				Power = power;
				WeaponOdds = odds;
				WeaponBet = bet;
			}
		}

		public Data FindWeaponData(Data.WEAPON_TYPE type)
		{
			return WeaponDatas.Select(x => x.WeaponType == type) as Data;
		}
	}
}