using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Extension;

namespace RemotingTest
{
    class TestUpdater
    {
        private Regulus.Remoting.IAgent agent;
        private ITestReturn testReturn;

        public TestUpdater(Regulus.Remoting.IAgent agent)
        {
            // TODO: Complete member initialization
            this.agent = agent;
            agent.QueryProvider<ITestReturn>().Return += _Set ;
        }

        private void _Set(ITestReturn obj)
        {
            testReturn = obj;
        }

        

        internal async System.Threading.Tasks.Task<int> Run()
        {
            
            

            while (testReturn == null)
            {

            }

            var testInterface = await testReturn.Test(1, 2).ToTask();

            int eventAddResult = 0;
            testInterface.ReturnEvent += add_result => eventAddResult = add_result;
            var addResult = await testInterface.Add(2, 3).ToTask();
            return eventAddResult;
        }
    }
}
