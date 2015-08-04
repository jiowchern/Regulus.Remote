// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageTest.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the StageTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Play;

#endregion

namespace GameTest.FormulaTest
{
	[TestClass]
	public class StageTest
	{
		[TestMethod]
		public void TestPassStageTicketInspector()
		{
			var locks = new[]
			{
				new StageLock
				{
					Requires = new[]
					{
						1, 
						2
					}, 
					Stage = 3
				}
			};
			var sg = new StageGate(locks);
			var sti = new StageTicketInspector(sg);
			sti.Initial(new[]
			{
				new Stage
				{
					Id = 1, 
					Pass = false
				}, 
				new Stage
				{
					Id = 2, 
					Pass = false
				}, 
				new Stage
				{
					Id = 4, 
					Pass = false
				}
			});

			sti.Pass(1);

			Assert.AreEqual(true, sti.PlayableStages.Any(Stage => Stage == 1));
			Assert.AreEqual(true, sti.PlayableStages.Any(Stage => Stage == 2));
			Assert.AreEqual(false, sti.PlayableStages.Any(Stage => Stage == 3));

			sti.Pass(2);

			Assert.AreEqual(true, sti.PlayableStages.Any(Stage => Stage == 1));
			Assert.AreEqual(true, sti.PlayableStages.Any(Stage => Stage == 2));
			Assert.AreEqual(true, sti.PlayableStages.Any(Stage => Stage == 3));
		}

		[TestMethod]
		public void TestKillStageTicketInspector()
		{
			var locks = new[]
			{
				new StageLock
				{
					KillCount = 10, 
					Stage = 3
				}
			};
			var sg = new StageGate(locks);
			var sti = new StageTicketInspector(sg);
			sti.Initial(new[]
			{
				new Stage
				{
					Id = 1, 
					Pass = false
				}, 
				new Stage
				{
					Id = 2, 
					Pass = false
				}
			});

			sti.Kill(1);
			sti.Pass(2);

			Assert.AreEqual(true, sti.PlayableStages.Any(Stage => Stage == 1));
			Assert.AreEqual(true, sti.PlayableStages.Any(Stage => Stage == 2));
			Assert.AreEqual(false, sti.PlayableStages.Any(Stage => Stage == 3));

			sti.Kill(9);

			Assert.AreEqual(true, sti.PlayableStages.Any(Stage => Stage == 1));
			Assert.AreEqual(true, sti.PlayableStages.Any(Stage => Stage == 2));
			Assert.AreEqual(true, sti.PlayableStages.Any(Stage => Stage == 3));
		}
	}
}