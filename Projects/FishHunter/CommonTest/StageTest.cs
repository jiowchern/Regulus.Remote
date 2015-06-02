using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VGame.Project.FishHunter
{
    [TestClass]
    public class StageTest
    {
        [TestMethod]
        public void TestStageTicketInspector()
        {
            var locks = new Data.StageLock[] { new Data.StageLock { Requires = new int[] {1,2} , Stage = 3} };
            var sg = new VGame.Project.FishHunter.Play.StageGate(locks);
            var sti = new VGame.Project.FishHunter.Play.StageTicketInspector(sg);
            sti.Initial(new Data.Stage[] { new Data.Stage { Id = 1, Pass = false }, new Data.Stage { Id = 2, Pass = false } });

            sti.Pass(1);

            Assert.AreEqual(true, sti.PlayableStages.Any(Stage => Stage == 1));
            Assert.AreEqual(true, sti.PlayableStages.Any(Stage => Stage == 2));
            Assert.AreEqual(false, sti.PlayableStages.Any(Stage => Stage == 3));

            sti.Pass(2);

            Assert.AreEqual(true, sti.PlayableStages.Any(Stage => Stage == 1));
            Assert.AreEqual(true, sti.PlayableStages.Any(Stage => Stage == 2));
            Assert.AreEqual(true, sti.PlayableStages.Any(Stage => Stage == 3));
            
        }
    }
}
