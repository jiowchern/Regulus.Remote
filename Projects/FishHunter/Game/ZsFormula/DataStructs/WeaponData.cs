// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeaponData.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the WeaponDataTable type.
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
	public class WeaponOddsRuler
	{
		public int CheckOddsRule(WeaponDataTable.Data data)
		{
			if ((int)data.WeaponType != 1)
			{
				var randNumber = Random.Instance.NextInt(0, 1000);
				if (randNumber < 750)
				{
					return 1;
				}
			}

			return 0;
		}
	}

	public class WeaponDataTable
	{
		public List<Data> WeaponDatas;

		public WeaponDataTable()
		{
			WeaponDatas = new List<Data>
			{
				new Data(Data.WEAPON_TYPE.BOMB_2, Data.WEAPON_POWER.BOMB_2, 100, 100, 3), 
				new Data(Data.WEAPON_TYPE.ELECTRIC_GRID_3, Data.WEAPON_POWER.ELECTRIC_GRID_3, 100, 100, 4), 
				new Data(Data.WEAPON_TYPE.TYPE_102, Data.WEAPON_POWER.FREE_1, 100, 100, 5), 
				new Data(Data.WEAPON_TYPE.FREE_NORMAL_1, Data.WEAPON_POWER.FREE_1, 100, 100, 5)
			};
		}

		public class Data
		{
			public enum WEAPON_POWER
			{
				FREE_1 = 120, // 免费炮

				BOMB_2 = 250, // 炸弹

				ELECTRIC_GRID_3 = 150 // 电网
			}

			public enum WEAPON_TYPE
			{
				FREE_NORMAL_1 = 1, 

				BOMB_2 = 2, 

				ELECTRIC_GRID_3 = 3, 

				FREE_NORMAL_4 = 4, 

				BOMB_101 = 101, // 海綿寶寶

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

			public int SpecialID { get; set; }

			public Data(WEAPON_TYPE type, WEAPON_POWER power, int odds, int bet, int special_id)
			{
				WeaponType = type;
				Power = power;
				WeaponOdds = odds;
				WeaponBet = bet;
				SpecialID = special_id;
			}
		}

		public Data FindWeaponData(Data.WEAPON_TYPE type)
		{
			return WeaponDatas.Select(x => x.WeaponType == type) as Data;
		}
	}
}