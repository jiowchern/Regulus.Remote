using System.Linq;


using Microsoft.VisualStudio.TestTools.UnitTesting;


using Regulus.Game;


using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;


using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace GameTest.ZsFormulaTest
{
	[Binding]
	[Scope(Feature = "ZsBetRangeTestSteps")]
	public class ZsBetRangeTestSteps
	{
		private BetChancesTable _BetChancesTable;

		[Given(@"buffer資料是")]
		public void GivenBuffer資料是(Table table)
		{
			var datas = table.CreateSet<ChancesTable<int>.Data>().ToArray();
			_BetChancesTable = new BetChancesTable(datas);
		}

		[When(@"最大押分是(.*)")]
		public void When最大押分是(int max_bet)
		{
			_BetChancesTable.MaxBet = max_bet;
		}

		[When(@"玩家押分是(.*)")]
		public void When玩家押分是(int player_bet)
		{
			_BetChancesTable.PlayerBet = player_bet;
		}

		[Then(@"取得的Buffer是")]
		public void Then取得的Buffer是(Table table)
		{
			var data = table.CreateInstance<ChancesTable<int>.Data>();

			Assert.AreEqual(data.Key, _BetChancesTable.GetDiceKey());
		}
	}
}
