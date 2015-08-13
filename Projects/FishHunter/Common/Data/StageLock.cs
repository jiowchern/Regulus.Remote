namespace VGame.Project.FishHunter.Common.Data
{
	public class StageLock
	{
		public int[] Requires { get; set; }

		public int KillCount { get; set; }

		public int Stage { get; set; }

		public StageLock()
		{
			Requires = new int[0];
		}
	}
}
