// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageLock.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the StageLock type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VGame.Project.FishHunter.Common.Data
{
	public class StageLock
	{
		public int[] Requires { get; set; }

		public int KillCount { get; set; }

		public int Stage { get; set; }

		public StageLock()
		{
			this.Requires = new int[0];
		}
	}
}