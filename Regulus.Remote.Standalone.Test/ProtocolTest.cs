using NUnit.Framework;
using Regulus.Projects.TestProtocol.Common;
using Regulus.Remote.Reactive;
using System;
using System.Collections;
using System.Reactive.Linq;

namespace Regulus.Remote.Standalone.Test
{
    public class ProtocolTest
    {
        /*[Test]
        public void Sample2NotifierTest()
        {
            var env = new SampleTestEnv();
            var queryer = env.Queryable;
            var obs = from sample in queryer.QueryNotifier<ISample>().SupplyEvent()
                      from add1 in System.Reactive.Linq.Observable.Defer<int>(() =>
                      {
                          env.Sample.Numbers.Items.Add(new Number(1));
                          return System.Reactive.Linq.Observable.Return(env.Sample.Numbers.Items.Count);
                      })
                      from numbers1 in sample.Numbers.SupplyEvent().Buffer(1).FirstAsync()
                      from add2 in System.Reactive.Linq.Observable.Defer<int>(() =>
                      {
                          env.Sample.Numbers.Items.Add(new Number(2));
                          return System.Reactive.Linq.Observable.Return(env.Sample.Numbers.Items.Count);
                      })
                      from numbers2 in sample.Numbers.SupplyEvent().Buffer(2).FirstAsync()
                      select new { numbers1, numbers2 };

            var testResult = obs.FirstAsync().Wait();


            env.Dispose();

            NUnit.Framework.Assert.AreEqual(1, testResult.numbers1[0].Value.Value);
            NUnit.Framework.Assert.AreEqual(1, testResult.numbers2[0].Value.Value);
            NUnit.Framework.Assert.AreEqual(2, testResult.numbers2[1].Value.Value);
        }*/
        
        


        [Test]
        public void SampleEventTest()
        {
            var env = new SampleTestEnv();
            var queryer = env.Queryable;


            var obs = from sample in queryer.QueryNotifier<ISample>().SupplyEvent()
                         from numberCount in System.Reactive.Linq.Observable.FromEvent<int>(h=> sample.IntsEvent += h , h => sample.IntsEvent -= h)
                         select numberCount;
            env.Sample.Ints.Items.Add(1);
            var testResult = obs.Buffer(1).FirstAsync().Wait();

            env.Dispose();
            
            NUnit.Framework.Assert.AreEqual(1, testResult[0]);            
        }

        [Test]
        public void Sample2EventTest()
        {
            var env = new SampleTestEnv();
            var queryer = env.Queryable;


            var obs = from sample in queryer.QueryNotifier<ISample>().SupplyEvent()
                       from add1 in System.Reactive.Linq.Observable.Defer<int>(() => { env.Sample.Ints.Items.Add(1); 
                                                                                    return System.Reactive.Linq.Observable.Return(env.Sample.Ints.Items.Count);
                                                                                })
                       from int1s in System.Reactive.Linq.Observable.FromEvent<int>(h => sample.IntsEvent += h, h => sample.IntsEvent -= h).Buffer(1).FirstAsync()
                       from add2 in System.Reactive.Linq.Observable.Defer<int>(() => {
                           env.Sample.Ints.Items.Add(2);
                           return System.Reactive.Linq.Observable.Return(env.Sample.Ints.Items.Count);
                       })
                       from int2s in System.Reactive.Linq.Observable.FromEvent<int>(h => sample.IntsEvent += h, h => sample.IntsEvent -= h).Buffer(2).FirstAsync()
                       select new { int1s,int2s };
            
            var testResult = obs.FirstAsync().Wait();

            

            env.Dispose();

            NUnit.Framework.Assert.AreEqual(1, testResult.int1s[0]);
            NUnit.Framework.Assert.AreEqual(1, testResult.int2s[0]);
            NUnit.Framework.Assert.AreEqual(2, testResult.int2s[1]);
        }

        

        [Test]
        public void SampleAddTest()
        {
            var env = new SampleTestEnv();
            var queryer = env.Queryable;
            var addObs = from sample in queryer.QueryNotifier<ISample>().SupplyEvent()
                         from result in sample.Add(1, 2).RemoteValue()
                         select result;

            int testResult = addObs.FirstAsync().Wait();

            env.Dispose();
            NUnit.Framework.Assert.AreEqual(3, testResult);
        }
    }
}
