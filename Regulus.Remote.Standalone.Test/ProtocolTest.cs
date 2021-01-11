using Xunit;
using Regulus.Projects.TestProtocol.Common;
using Regulus.Remote.Reactive;
using System;
using System.Collections;
using System.Reactive.Linq;

namespace Regulus.Remote.Standalone.Test
{
    public class ProtocolTest
    {
        
        
        public async System.Threading.Tasks.Task AllInOne()
        {
            await Sample2NotifierUnsupplyTest();
            await Sample2NotifierSupplyTest();
            await SampleEventTest();
            await Sample2EventTest();
            await SampleAddTest();
        }
        [Xunit.Fact(Timeout = 10000)]
        public async System.Threading.Tasks.Task Sample2NotifierUnsupplyTest()
        {
            var env = new SampleTestEnv();


            var n1 = new Number(1);
            var n2 = new Number(2);
            var n3 = new Number(3);
            env.Sample.Numbers.Items.Add(n1);
            env.Sample.Numbers.Items.Add(n2);
            env.Sample.Numbers.Items.Add(n3);
            var queryer = env.Queryable;
            var readyObs = from s in queryer.QueryNotifier<ISample>().SupplyEvent()
                           from supplyNumbers in s.Numbers.SupplyEvent().Buffer(3)                      
                           select s ;
            var sample = await readyObs.FirstAsync();

            var numbers = new System.Collections.Concurrent.ConcurrentQueue<INumber>();
            sample.Numbers.Unsupply += numbers.Enqueue;


            var removeObs = from r1 in sample.RemoveNumber(2).RemoteValue()
                            from r2 in sample.RemoveNumber(1).RemoteValue()
                            from r3 in sample.RemoveNumber(3).RemoteValue()
                            select new { r1, r2, r3 };

            var removeNumbers = await removeObs.FirstAsync();

            

            env.Dispose();

            INumber number1;
            numbers.TryDequeue(out number1);
            INumber number2;
            numbers.TryDequeue(out number2);
            INumber number3;
            numbers.TryDequeue(out number3);
            Xunit.Assert.Equal(2 , number1.Value.Value);
            Xunit.Assert.Equal(1, number2.Value.Value);
            Xunit.Assert.Equal(3, number3.Value.Value);
        }
        [Xunit.Fact(Timeout = 10000)]
        public async System.Threading.Tasks.Task Sample2NotifierSupplyTest()
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

            
            var testResult =  await obs.FirstAsync();


            env.Dispose();

            Xunit.Assert.Equal(1, testResult.numbers1[0].Value.Value);
            Xunit.Assert.Equal(1, testResult.numbers2[0].Value.Value);
            Xunit.Assert.Equal(2, testResult.numbers2[1].Value.Value);
        }



        [Xunit.Fact(Timeout = 10000)]
        public async System.Threading.Tasks.Task SampleEventTest()
        {
            var env = new SampleTestEnv();
            var queryer = env.Queryable;


            var obs = from sample in queryer.QueryNotifier<ISample>().SupplyEvent()
                         from numberCount in System.Reactive.Linq.Observable.FromEvent<int>(h=> sample.IntsEvent += h , h => sample.IntsEvent -= h)
                         select numberCount;
            env.Sample.Ints.Items.Add(1);
            var testResult = await obs.Do((v)=> { },_Throw).FirstAsync();

            env.Dispose();
            
            Xunit.Assert.Equal(1, testResult);            
        }

        [Xunit.Fact(Timeout = 10000)]
        public async System.Threading.Tasks.Task Sample2EventTest()
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
            
            var testResult = await obs.FirstAsync();

            

            env.Dispose();

            Xunit.Assert.Equal(1, testResult.int1s[0]);
            Xunit.Assert.Equal(1, testResult.int2s[0]);
            Xunit.Assert.Equal(2, testResult.int2s[1]);
        }



        [Xunit.Fact(Timeout = 10000)]
        public async System.Threading.Tasks.Task SampleAddTest()
        {
            var env = new SampleTestEnv();
            var queryer = env.Queryable;
            var addObs = from sample in queryer.QueryNotifier<ISample>().SupplyEvent()
                         from result in sample.Add(1, 2).RemoteValue()
                         select result;
            
            int verifyResult = await addObs.Do((r) => { }, _Throw).FirstAsync();
            
            env.Dispose();
            Xunit.Assert.Equal(3, verifyResult);
        }

        private void _Throw(Exception e)
        {
            throw e;
        }
    }
}
