using Xunit;
using Regulus.Projects.TestProtocol.Common;
using Regulus.Remote.Reactive;
using System;
using System.Linq;
using System.Collections;
using System.Reactive.Linq;

namespace Regulus.Remote.Standalone.Test
{
    public class ProtocolTest
    {

        
        [Xunit.Theory()]
        [Xunit.InlineData(1)]
        public void AllInOneCount(int count)
        {
            var tasks = from _ in System.Linq.Enumerable.Range(0, count)
                        select AllInOne();

            System.Threading.Tasks.Task.WhenAll(tasks);


        }
        //[Xunit.Fact()]
        public async System.Threading.Tasks.Task AllInOne()
        {
            await Sample2NotifierUnsupplyTest();
            await Sample2NotifierSupplyTest();
            await SampleEventTest();
            await Sample2EventTest();
            await SampleAddTest();
        }
        [Xunit.Fact()]
        public async System.Threading.Tasks.Task Sample2NotifierUnsupplyTest()
        {
            var env = new TestEnv<SampleEntry>(new SampleEntry());

            var n1 = new Number(1);
            var n2 = new Number(2);
            var n3 = new Number(3);
            
            env.Entry.Sample.Numbers.Items.Add(n1);
            env.Entry.Sample.Numbers.Items.Add(n2);
            env.Entry.Sample.Numbers.Items.Add(n3);
            var queryer = env.Queryable;
            var readyObs = from s in queryer.QueryNotifier<ISample>().SupplyEvent()
                           from supplyNumbers in s.Numbers.SupplyEvent().Buffer(3)                      
                           select s ;
            var sample = await readyObs.FirstAsync();

            var numbers = new System.Collections.Concurrent.ConcurrentQueue<INumber>();
            sample.Numbers.Base.Unsupply += numbers.Enqueue;


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
        [Xunit.Fact()]
        public async System.Threading.Tasks.Task Sample2NotifierSupplyTest()
        {
            var env = new TestEnv<SampleEntry>(new SampleEntry());
            var queryer = env.Queryable;
            var obs1 = from sample in queryer.QueryNotifier<ISample>().SupplyEvent()
                      from add1 in System.Reactive.Linq.Observable.Defer<int>(() =>
                      {
                          env.Entry.Sample.Numbers.Items.Add(new Number(1));
                          return System.Reactive.Linq.Observable.Return(env.Entry.Sample.Numbers.Items.Count);
                      })
                      from numbers1 in sample.Numbers.SupplyEvent().Buffer(1)                      
                      select numbers1;
           

            var testResult1 = await obs1.FirstAsync();

            var obs2 =
                from sample in queryer.QueryNotifier<ISample>().SupplyEvent()
                from add2 in System.Reactive.Linq.Observable.Defer<int>(() =>
                {
                    env.Entry.Sample.Numbers.Items.Add(new Number(2));
                    return System.Reactive.Linq.Observable.Return(env.Entry.Sample.Numbers.Items.Count);
                })
                from numbers2 in sample.Numbers.SupplyEvent().Buffer(2)
                select numbers2;

            var testResult2 = await obs2.FirstAsync();

            env.Dispose();
            
            Xunit.Assert.Equal(1, testResult1[0].Value.Value);
            Xunit.Assert.Equal(1, testResult2[0].Value.Value);
            Xunit.Assert.Equal(2, testResult2[1].Value.Value);
        }



        [Xunit.Fact()]
        public async System.Threading.Tasks.Task SampleEventTest()
        {
            var env = new TestEnv<SampleEntry>(new SampleEntry());
            var queryer = env.Queryable;


            var obs = from sample in queryer.QueryNotifier<ISample>().SupplyEvent()
                         from numberCount in System.Reactive.Linq.Observable.FromEvent<int>(h=> sample.IntsEvent += h , h => sample.IntsEvent -= h)
                         select numberCount;
            env.Entry.Sample.Ints.Items.Add(1);
            var testResult = await obs.Do((v)=> { },_Throw).FirstAsync();

            env.Dispose();
            
            Xunit.Assert.Equal(1, testResult);            
        }

        [Xunit.Fact()]
        public async System.Threading.Tasks.Task Sample2EventTest()
        {
            var env = new TestEnv<SampleEntry>(new SampleEntry());
            var queryer = env.Queryable;


            var obs = from sample in queryer.QueryNotifier<ISample>().SupplyEvent()
                       from add1 in System.Reactive.Linq.Observable.Defer<int>(() => { env.Entry.Sample.Ints.Items.Add(1); 
                                                                                    return System.Reactive.Linq.Observable.Return(env.Entry.Sample.Ints.Items.Count);
                                                                                })
                       from int1s in System.Reactive.Linq.Observable.FromEvent<int>(h => sample.IntsEvent += h, h => sample.IntsEvent -= h).Buffer(1)
                       from add2 in System.Reactive.Linq.Observable.Defer<int>(() => {
                           env.Entry.Sample.Ints.Items.Add(2);
                           return System.Reactive.Linq.Observable.Return(env.Entry.Sample.Ints.Items.Count);
                       })
                       from int2s in System.Reactive.Linq.Observable.FromEvent<int>(h => sample.IntsEvent += h, h => sample.IntsEvent -= h).Buffer(2)
                       select new { int1s,int2s };
            
            var testResult = await obs.FirstAsync();

            

            env.Dispose();

            Xunit.Assert.Equal(1, testResult.int1s[0]);
            Xunit.Assert.Equal(1, testResult.int2s[0]);
            Xunit.Assert.Equal(2, testResult.int2s[1]);
        }



        [Xunit.Fact()]
        public async System.Threading.Tasks.Task SampleAddTest()
        {
            var env = new TestEnv<SampleEntry>(new SampleEntry());
            var queryer = env.Queryable;
            var addObs = from sample in queryer.QueryNotifier<ISample>().SupplyEvent()
                         from result in sample.Add(1, 2).RemoteValue()
                         select result;
            
            int verifyResult = await addObs.Do((r) => { }, _Throw).FirstAsync();
            
            env.Dispose();
            
            Xunit.Assert.Equal(3, verifyResult);
        }

        [Xunit.Fact(Timeout = 10000)]
        public async System.Threading.Tasks.Task StatusCleanRelease()
        {
            
            var env = new TestEnv<Sample.UpdatableEntry>(new Sample.UpdatableEntry( binder => new StatusEntryUser(binder)));

            var supplyNumValuesObs = from sample in env.Queryable.QueryNotifier<ISample>().SupplyEvent()
                            from num in sample.Numbers.SupplyEvent()
                            select num.Value.Value;
            var supplyNumValues = await supplyNumValuesObs.Buffer(3).FirstAsync();

            var unsupplyNumValuesObs = from sample in env.Queryable.QueryNotifier<ISample>().SupplyEvent()                                       
                                       from num in sample.Numbers.UnsupplyEvent()
                                     select num.Value.Value;

            System.Collections.Generic.List<int> values = new System.Collections.Generic.List<int>();
            unsupplyNumValuesObs.Buffer(3).Subscribe(values.AddRange);
            
            var nextObs = from next in env.Queryable.QueryNotifier<INext>().SupplyEvent()
                            from _ in next.Next().RemoteValue()
                            select _;
            await nextObs.FirstAsync();
            var ar = new Regulus.Utility.AutoPowerRegulator(new Utility.PowerRegulator());
            while (values.Count == 0)
            {
                ar.Operate();
            }

            env.Dispose();
            Xunit.Assert.Equal(supplyNumValues.Count , values.Count);
        }

        private void _Throw(Exception e)
        {
            throw e;
        }
    }
}
