using System;
using System.Linq;
using System.Reactive.Linq;


using NUnit.Framework;
using Regulus.Remote.Reactive;
using Regulus.Remote.Tools.Protocol.Sources.TestCommon.MultipleNotices;

namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            
        }
        [Test]
        public void CreateProtocolTest1()
        {

            var protocol = Regulus.Remote.Tools.Protocol.Sources.TestCommon.ProtocolProvider.CreateCase1();
            NUnit.Framework.Assert.IsNotNull(protocol);
        }

        [Test]
        public void CreateProtocolTest2()
        {
            var protocol = Regulus.Remote.Tools.Protocol.Sources.TestCommon.ProtocolProvider.CreateCase2();
            NUnit.Framework.Assert.IsNotNull(protocol);
        }

        [Test]
        public void CreateProtocolSerializeTypesTest()
        {
            var protocol = Regulus.Remote.Tools.Protocol.Sources.TestCommon.ProtocolProvider.CreateCase1();
            NUnit.Framework.Assert.IsTrue(protocol.SerializeTypes.Any(t => t == typeof(int)));

            NUnit.Framework.Assert.AreEqual(13, protocol.SerializeTypes.Length);


        }

        [Test]
        public void CreateProtocolTest3()
        {
            var protocol = ProtocolProviderCase3.CreateCase3();
            NUnit.Framework.Assert.IsNotNull(protocol);
        }
        [Test]
        public void NotifierSupplyAndUnsupplyTest()
        {
            var multipleNotices = new MultipleNotices.MultipleNotices();

            var env = new TestEnv<Entry<IMultipleNotices>, IMultipleNotices>(new Entry<IMultipleNotices>(multipleNotices));

            var n1 = new Regulus.Remote.Tools.Protocol.Sources.TestCommon.Number(1);
            var n2 = new Regulus.Remote.Tools.Protocol.Sources.TestCommon.Number(2);
            var n3 = new Regulus.Remote.Tools.Protocol.Sources.TestCommon.Number(3);

            multipleNotices.Numbers1.Items.Add(n1);
            multipleNotices.Numbers1.Items.Add(n2);
            multipleNotices.Numbers1.Items.Add(n2);
            multipleNotices.Numbers1.Items.Add(n3);

            multipleNotices.Numbers2.Items.Add(n2);
            multipleNotices.Numbers2.Items.Add(n3);

            var supplyn1Obs = from mn in env.Queryable.QueryNotifier<IMultipleNotices>().SupplyEvent()
                              from n in mn.Numbers1.Base.SupplyEvent()
                              select n.Value.Value;

            var supplyn2Obs = from mn in env.Queryable.QueryNotifier<IMultipleNotices>().SupplyEvent()
                              from n in mn.Numbers2.Base.SupplyEvent()
                              select n.Value.Value;

            var unsupplyn1Obs = from mn in env.Queryable.QueryNotifier<IMultipleNotices>().SupplyEvent()
                                from n in mn.Numbers1.Base.UnsupplyEvent()
                                select n.Value.Value;

            var unsupplyn2Obs = from mn in env.Queryable.QueryNotifier<IMultipleNotices>().SupplyEvent()
                                from n in mn.Numbers2.Base.UnsupplyEvent()
                                select n.Value.Value;



            var num1s = supplyn1Obs.Buffer(4).FirstAsync().Wait();
            var num2s = supplyn2Obs.Buffer(2).FirstAsync().Wait();



            NUnit.Framework.Assert.AreEqual(1, num1s[0]);
            NUnit.Framework.Assert.AreEqual(2, num1s[1]);
            NUnit.Framework.Assert.AreEqual(2, num1s[2]);
            NUnit.Framework.Assert.AreEqual(3, num1s[3]);
            NUnit.Framework.Assert.AreEqual(2, num2s[0]);
            NUnit.Framework.Assert.AreEqual(3, num2s[1]);

            var removeNum1s = new System.Collections.Generic.List<int>();
            unsupplyn1Obs.Subscribe(removeNum1s.Add);
            unsupplyn2Obs.Subscribe(removeNum1s.Add);

            multipleNotices.Numbers1.Items.Remove(n2);
            multipleNotices.Numbers2.Items.Remove(n2);
            var c1 = multipleNotices.Numbers1.Items.Count;
            var c2 = multipleNotices.Numbers2.Items.Count;



            System.Threading.SpinWait.SpinUntil(() => removeNum1s.Count == 2, 5000);
            NUnit.Framework.Assert.AreEqual(2, removeNum1s[0]);
            NUnit.Framework.Assert.AreEqual(2, removeNum1s[1]);

            env.Dispose();
        }
        [Test]
        public void NotifierSupplyTest()
        {


            var multipleNotices = new MultipleNotices.MultipleNotices();

            var env = new TestEnv<Entry<IMultipleNotices>, IMultipleNotices>(new Entry<IMultipleNotices>(multipleNotices));

            var n1 = new Regulus.Remote.Tools.Protocol.Sources.TestCommon.Number(1);


            multipleNotices.Numbers1.Items.Add(n1);
            multipleNotices.Numbers1.Items.Add(n1);
            multipleNotices.Numbers2.Items.Add(n1);

            var supplyn1Obs = from mn in env.Queryable.QueryNotifier<IMultipleNotices>().SupplyEvent()
                              from n in mn.Numbers1.Base.SupplyEvent()
                              select n.Value.Value;

            var supplyn2Obs = from mn in env.Queryable.QueryNotifier<IMultipleNotices>().SupplyEvent()
                              from n in mn.Numbers2.Base.SupplyEvent()
                              select n.Value.Value;

            var unsupplyn1Obs = from mn in env.Queryable.QueryNotifier<IMultipleNotices>().SupplyEvent()
                                from n in mn.Numbers1.Base.UnsupplyEvent()
                                select n.Value.Value;

            var unsupplyn2Obs = from mn in env.Queryable.QueryNotifier<IMultipleNotices>().SupplyEvent()
                                from n in mn.Numbers2.Base.UnsupplyEvent()
                                select n.Value.Value;

            var num1s = supplyn1Obs.Buffer(2).FirstAsync().Wait();
            var num2s = supplyn2Obs.Buffer(1).FirstAsync().Wait();

            NUnit.Framework.Assert.AreEqual(1, num1s[0]);
            NUnit.Framework.Assert.AreEqual(1, num1s[1]);
            NUnit.Framework.Assert.AreEqual(1, num2s[0]);

            var removeNums = new System.Collections.Generic.List<int>();

            unsupplyn1Obs.Subscribe(removeNums.Add);
            unsupplyn2Obs.Subscribe(removeNums.Add);

            var count1Obs = from mn in env.Queryable.QueryNotifier<IMultipleNotices>().SupplyEvent()
                            from count in mn.GetNumber1Count().RemoteValue()
                            select count;

            var count2Obs = from mn in env.Queryable.QueryNotifier<IMultipleNotices>().SupplyEvent()
                            from count in mn.GetNumber2Count().RemoteValue()
                            select count;



            multipleNotices.Numbers2.Items.Remove(n1);
            multipleNotices.Numbers1.Items.Remove(n1);
            multipleNotices.Numbers1.Items.Remove(n1);

            var count1 = count1Obs.FirstAsync().Wait();
            var count2 = count2Obs.FirstAsync().Wait();

            NUnit.Framework.Assert.AreEqual(0, count1);
            NUnit.Framework.Assert.AreEqual(0, count2);


            System.Threading.SpinWait.SpinUntil(() => removeNums.Count == 3, 5000);
            NUnit.Framework.Assert.AreEqual(1, removeNums[0]);
            NUnit.Framework.Assert.AreEqual(1, removeNums[1]);
            NUnit.Framework.Assert.AreEqual(1, removeNums[2]);


            env.Dispose();
        }

        [Test]
        public void EventCustomDelegateTest()
        {
            var tester = new EventTester();


            var env = new TestEnv<Entry<IEventabe>, IEventabe>(new Entry<IEventabe>(tester));

            var eventerObs = from e in env.Queryable.QueryNotifier<IEventabe>().SupplyEvent()
                             select e;

            var eventer = eventerObs.FirstAsync().Wait();

            CustomDelegate testAction = () => { };

            NUnit.Framework.Assert.Throws<Regulus.Remote.Exceptions.NotSupportedException>(() => eventer.CustomDelegateEvent += testAction);
            
            

        }

            [Test]
        public void EventRemoveTest()
        {
            var tester = new EventTester();

            
            var env = new TestEnv<Entry<IEventabe>, IEventabe>(new Entry<IEventabe>(tester));

            var eventerObs = from e in env.Queryable.QueryNotifier<IEventabe>().SupplyEvent()
                             select e;

            var eventer = eventerObs.FirstAsync().Wait();

            System.Action actionEmpty = () => { };
            eventer.Event02 += actionEmpty;
            System.Threading.SpinWait.SpinUntil(() => tester.Event02AddCount == 1, 5000);

            eventer.Event02 -= actionEmpty;
            System.Threading.SpinWait.SpinUntil(() => tester.Event02RemoveCount == 1, 5000);



            env.Dispose();

            NUnit.Framework.Assert.AreEqual(1, tester.Event02AddCount);
            NUnit.Framework.Assert.AreEqual(1, tester.Event02RemoveCount);
        }

        [Test]
        public void EventTest()
        {
            var tester = new EventTester();

            
            var env = new TestEnv<Entry<IEventabe>, IEventabe>(new Entry<IEventabe>(tester));



            var event11Obs = from eventer in env.Queryable.QueryNotifier<IEventabe>().SupplyEvent()
                             from n in Reactive.Extensions.EventObservable((h) => eventer.Event1 += h, (h) => eventer.Event1 -= h)
                             select n;
            var event12Obs = from eventer in env.Queryable.QueryNotifier<IEventabe>().SupplyEvent()
                             from n in Reactive.Extensions.EventObservable((h) => eventer.Event21 += h, (h) => eventer.Event21 -= h)
                             select n;

            var event21Obs = from eventer in env.Queryable.QueryNotifier<IEventabe>().SupplyEvent()
                             from n in Reactive.Extensions.EventObservable<int>((h) => eventer.Event2 += h, (h) => eventer.Event2 -= h)
                             select n;
            var event22Obs = from eventer in env.Queryable.QueryNotifier<IEventabe>().SupplyEvent()
                             from n in Reactive.Extensions.EventObservable<int>(
                                 (h) => eventer.Event22 += h,
                                 (h) => eventer.Event22 -= h)
                             select n;

            var vals = new System.Collections.Generic.List<int>();
            event11Obs.Subscribe((unit) => vals.Add(1));
            event12Obs.Subscribe((unit) => vals.Add(2));
            event21Obs.Subscribe(vals.Add);
            event22Obs.Subscribe(vals.Add);


            System.Console.WriteLine("wait EventTest tester.LisCount ...");
            System.Threading.SpinWait.SpinUntil(() => tester.LisCount == 4, 5000);
            

            tester.Invoke22(9);
            tester.Invoke21();
            tester.Invoke11();
            tester.Invoke12(8);


            System.Console.WriteLine("wait EventTest vals.Count ...");

            System.Threading.SpinWait.SpinUntil(() => vals.Count == 4, 5000);
            

            env.Dispose();

            NUnit.Framework.Assert.AreEqual(9, vals[0]);
            NUnit.Framework.Assert.AreEqual(2, vals[1]);
            NUnit.Framework.Assert.AreEqual(1, vals[2]);
            NUnit.Framework.Assert.AreEqual(8, vals[3]);



        }


        [Test]
        public void MethodNotSupportedTest()
        {
            var tester = new MethodTester();

            var env = new TestEnv<Entry<IMethodable>, IMethodable>(new Entry<IMethodable>(tester));
            var gpiObs = from g in env.Queryable.QueryNotifier<IMethodable>().SupplyEvent()
                         select g;

            var gpi= gpiObs.FirstAsync().Wait();
            try
            {
                gpi.NotSupported();
            }
            catch (Regulus.Remote.Exceptions.NotSupportedException sue)
            {

                NUnit.Framework.Assert.Pass();
                return;
            }

            NUnit.Framework.Assert.Fail();
        }

            [Test]
        public void MethodTest()
        {


            var tester = new MethodTester();
            
            var env = new TestEnv<Entry<IMethodable>, IMethodable>(new Entry<IMethodable>(tester));
            var valuesObs = from gpi in env.Queryable.QueryNotifier<IMethodable>().SupplyEvent()
                             from v1 in gpi.GetValue1().RemoteValue()
                             from v2 in gpi.GetValue2().RemoteValue()
                             from v0 in gpi.GetValue0(0,"",0,0,0,Guid.Empty).RemoteValue()
                            select new {v1,v2,v0};

            var values = valuesObs.FirstAsync().Wait();
            env.Dispose();

            Assert.AreEqual(1, values.v1);
            Assert.AreEqual(2, values.v2);
            Assert.AreEqual(0, values.v0[0]);
        }

        [Test , Timeout(1000*60)]
        public void MethodReturnTypeTest()
        {


            var tester = new MethodTester();

            var env = new TestEnv<Entry<IMethodable>, IMethodable>(new Entry<IMethodable>(tester));
            var methodObs = from gpi in env.Queryable.QueryNotifier<IMethodable>().SupplyEvent()
                            from v1 in gpi.GetValueSelf().RemoteValue()                            
                            select v1;

            var method = methodObs.FirstAsync().Wait();

            var valueObs = from v1 in method.GetValue1().RemoteValue()
                            select v1;

            var value = valueObs.FirstAsync().Wait();

            method = null;
            
            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();


            env.Dispose();


            Assert.AreEqual(1, value);            
        }

        [Test]
        public void MethodSayHelloTest()
        {


            var tester = new MethodTester();

            var env = new TestEnv<Entry<IMethodable>, IMethodable>(new Entry<IMethodable>(tester));
            var valuesObs = from gpi in env.Queryable.QueryNotifier<IMethodable>().SupplyEvent()
                            from response in gpi.SayHello(new HelloRequest() { Name = "jc"}).RemoteValue()
                            select response;

            var values = valuesObs.FirstAsync().Wait();
            env.Dispose();

            Assert.AreEqual("jc", values.Message);
            
        }

        [Test]
        public void PropertyTest()
        {
            var tester = new PropertyTester();
            var env = new TestEnv<Entry<IPropertyable>, IPropertyable>(new Entry<IPropertyable>(tester));
            
            var values1Obs = from gpi in env.Queryable.QueryNotifier<IPropertyable>().SupplyEvent()                            
                            select new { v1=gpi.Property1.Value,v2= gpi.Property2.Value };

            
            var values = values1Obs.FirstAsync().Wait();

            Assert.AreEqual(1, values.v1);
            Assert.AreEqual(2, values.v2);
            

            var values2Obs = from gpi in env.Queryable.QueryNotifier<IPropertyable>().SupplyEvent()
                             from v1 in gpi.Property1.PropertyChangeValue()
                             from v2 in gpi.Property2.PropertyChangeValue()
                             select new { v1 , v2};

            int[] changes = new int[] { 1, 2 };
            values2Obs.Subscribe(o => { changes[0] = o.v1; changes[1] = o.v2; });

            
            System.Threading.SpinWait.SpinUntil(() => {
                tester.Property1.Value ++ ;
                tester.Property2.Value ++;                
                return changes[0] != 1 && changes[1] != 2;
            } ,5000);
            
            Assert.AreNotEqual(1, changes[0]);
            Assert.AreNotEqual(2, changes[1]);

            env.Dispose();

            
        }

        
    }
}