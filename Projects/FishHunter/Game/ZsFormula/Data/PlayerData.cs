// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerData.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the Player type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

using VGame.Project.FishHunter.Common.Datas.FishStage;

namespace VGame.Project.FishHunter.ZsFormula.Data
{
	public class Player
	{
		private List<Data> _StageDatas;

		public class Data
		{
			public int Id { get; set; }

			public int StageId { get; set; }

			public int Status { get; set; }

			public int BufferValue { get; set; }

			public SpecialWeaponData NowSpecialWeaponData { get; set; }

			public StageRecode RecodeData { get; set; }

			public Data()
			{
				RecodeData = new StageRecode();
			}
		}

		public Data FindStageData(int stage_id)
		{
			return _StageDatas.Select(x => x.StageId == stage_id) as Data;
		}
	}
}