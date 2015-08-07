// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageDataTable.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the FishStageVisitor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;


using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.ZsFormula.Data
{
	public class FishStageVisitor
	{
		public StageData NowData { get; private set; }

		public StageBuffer.BUFFER_BLOCK NowBlock { get;  set; }

		public FishStageVisitor(StageData stage_data)
		{
			NowData = stage_data;
		}
	}
}

