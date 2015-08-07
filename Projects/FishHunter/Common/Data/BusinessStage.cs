
namespace VGame.Project.FishHunter.Common.Data
{
	 public class BusinessStage
	{
		public BusinessStage()
		{
			BusinessStage.StageIds = new byte[]
			{
				1,
				2,
				3
			};
		}

		 public static byte[] StageIds { get; private set; }
	}
}