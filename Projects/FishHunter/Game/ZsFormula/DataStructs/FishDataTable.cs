// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FishDataTable.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   00以上为特殊鱼（死亡后，有爆炸效果的），直接变成特殊武器
//   海绵宝宝x80、电鳗x150、贪食蛇x120、铁球x200、小章鱼x200、大章鱼xN（必死）
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System.Collections.Generic;

namespace VGame.Project.FishHunter.ZsFormula.DataStructs
{
	public interface IFishDataFinder
	{
		FishDataTable.Data FindFishData(FishDataTable.Data.FISH_TYPE type);
	}

	public class FishDataTable : IFishDataFinder
	{
		private readonly Dictionary<int, Data> _Datas;

		public FishDataTable()
		{
			_Datas = new Dictionary<int, Data>();
		}

		Data IFishDataFinder.FindFishData(Data.FISH_TYPE type)
		{
			return _Datas[(int)type];
		}

		public class Data
		{
			public enum FISH_TYPE
			{
				TYPE_1 = 0, 

				TYPE_2, 

				TYPE_3, 

				TYPE_4, 

				TYPE_5, 

				TYPE_6, 

				TYPE_7, 

				TYPE_8 = 8, 

				DEF_100 = 100, 

				DEF_100_A, 

				DEF_200_A, 

				DEF_400_A
			}

			public int Odds { get; set; }

			public FISH_TYPE FishType { get; private set; }

			public Data(FISH_TYPE fish_type, int odds)
			{
				FishType = fish_type;
				Odds = odds;
			}

			public Data()
			{
			}
		}
	}
}