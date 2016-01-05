using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule.FloatingOdds;

namespace GameTest.ZsFormulaTest
{
	[Binding]
	[Scope(Feature = "魚王擊中倍數")]
	public class 魚王擊中倍數Steps
	{
		private RequsetFishData[] _Ints;

		[Given(@"擊中魚的資料是")]
		public void Given擊中魚的資料是(Table table)
		{
			_Ints = table.CreateSet<RequsetFishData>()
									.ToArray();
		}

		[When(@"祭品是")]
		public void When祭品是(Table table)
		{
			_Ints[0].GraveGoods = table.CreateSet<RequsetFishData>()
													.ToArray();
		}

		[Then(@"加總倍數是 (.*)")]
		public void Then加總倍數是(int p0)
		{
			(new King() as IFloatingCalculator).Calculate(_Ints);

			var kings = _Ints.Where(x => x.FishStatus == FISH_STATUS.KING);

			foreach(var king in kings)
			{
				Assert.AreEqual(king.FishOdds, p0);
			}
		}
	}
}
