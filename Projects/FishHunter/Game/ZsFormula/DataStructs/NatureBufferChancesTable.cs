// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NatureBufferChancesTable.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the NatureBufferChancesTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Collections.Generic;
using System.Linq;

using Regulus.Game;

namespace VGame.Project.FishHunter.ZsFormula.DataStructs
{
	public class NatureBufferChancesTable : RangeChancesTable<int>
	{
		public Dictionary<int, Data> Datas { get; private set; }

		public NatureBufferChancesTable(Data[] datas) : base(datas)
		{
		}

		public Data Find(int id)
		{
			this.Datas = this._Datas.ToDictionary(x => x.Key);
			return this.Datas[id];
		}

		public int FindValue(int id)
		{
			var datas = this._Datas.ToDictionary(x => x.Key);
			return (int)datas[id].Value;
		}
	}
}