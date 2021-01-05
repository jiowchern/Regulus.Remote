using NUnit.Framework;
using NUnit.Framework.Constraints;
using Regulus.Remote.Reactive;
using System.Reactive.Linq;

namespace Regulus.Remote.Standalone.Test
{
    public class StandaloneTest
    {

        
        [Test]
        [MaxTime(5000)]
        public void Test()
        {

            var env = new SampleTestEnv();

            var obs = from sample in env.Queryable.QueryNotifier<Projects.TestProtocol.Common.ISample>().SupplyEvent()
                        select sample;
            var s = obs.FirstAsync().Wait();

            env.Dispose();


            NUnit.Framework.Assert.AreNotEqual(null , s);
            

        }

        

        
    }
}
