using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule.FloatingOdds;

namespace GameTest.ZsFormulaTest
{
	[Binding]
	[Scope(Feature = "大章魚擊中倍數")]
	public class 大章魚擊中倍數Steps
	{
		private RequsetFishData[] _Int;

		[Given(@"擊中魚的資料是")]
		public void Given擊中魚的資料是(Table table)
		{
			_Int = table.CreateSet<RequsetFishData>()
									.ToArray();
		}

		[When(@"祭品是")]
		public void When祭品是(Table table)
		{
			_Int[0].GraveGoods = table.CreateSet<RequsetFishData>()
												.ToArray();
		}

		[Then(@"加總倍數是 (.*)")]
		public void Then加總倍數是(int p0)
		{
			(new BigOctopus() as IFloatingCalculator).Calculate(_Int);

			var octs = _Int.Where(x => x.FishType == FISH_TYPE.SPECIAL_BIG_OCTOPUS_BOMB);

			foreach(var oct in octs)
			{
				Assert.AreEqual(oct.FishOdds, p0);
			}
		}
	}
}
