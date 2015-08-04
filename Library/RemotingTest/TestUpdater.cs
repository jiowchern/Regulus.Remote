// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestUpdater.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the TestUpdater type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Threading.Tasks;

using Regulus.Net45;
using Regulus.Remoting;

#endregion

namespace RemotingTest
{
	internal class TestUpdater
	{
		private IAgent agent;

		private ITestReturn testReturn;

		public TestUpdater(IAgent agent)
		{
			// TODO: Complete member initialization
			this.agent = agent;
			agent.QueryNotifier<ITestReturn>().Return += _Set;
		}

		private void _Set(ITestReturn obj)
		{
			testReturn = obj;
		}

		internal async Task<int> Run()
		{
			while (testReturn == null)
			{
			}

			var testInterface = await testReturn.Test(1, 2).ToTask();

			var eventAddResult = 0;
			testInterface.ReturnEvent += add_result => eventAddResult = add_result;
			var addResult = await testInterface.Add(2, 3).ToTask();
			return eventAddResult;
		}
	}
}