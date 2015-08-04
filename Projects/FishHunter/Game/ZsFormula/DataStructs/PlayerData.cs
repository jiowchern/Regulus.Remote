// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerData.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the Player type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Collections.Generic;
using System.Linq;

#endregion

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

			public RecodeData.SpecialWeaponData NowSpecialWeaponData { get; set; }

			public RecodeData RecodeData { get; set; }

			public Data()
			{
				this.RecodeData = new RecodeData();
			}
		}

		public Data FindStageData(int stage_id)
		{
			return _StageDatas.Select(x => x.StageId == stage_id) as Data;
		}
	}
}