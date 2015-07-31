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

namespace VGame.Project.FishHunter.ZsFormula.DataStructs
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

			public int Item { get; set; }

			public Recode Recode { get; set; }

			public Data()
			{
				Recode = new Recode();
			}
		}

		public Data FindStageData(int stage_id)
		{
			return _StageDatas.Select(x => x.StageId == stage_id) as Data;
		}
	}
}