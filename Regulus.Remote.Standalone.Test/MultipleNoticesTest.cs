//using Regulus.Projects.TestProtocol.Common.MultipleNotices;
using Regulus.Remote.Reactive;
using System;
using System.Linq;
using System.Reactive.Linq;



namespace Regulus.Remote.Standalone.Test
{
    /*public class MultipleNoticesTest
    {
        [Xunit.Fact()]

        public async System.Threading.Tasks.Task TestSupply()
        {
            var multipleNotices = new MultipleNotices();

            var env = new TestEnv<Entry<IMultipleNotices>>(new Entry<IMultipleNotices>(multipleNotices));

            var n1 = new Regulus.Projects.TestProtocol.Common.Number(1);
          
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


            var num1s = await supplyn1Obs.Buffer(2).FirstAsync();
            var num2s = await supplyn2Obs.Buffer(1).FirstAsync();

            Xunit.Assert.Equal(1, num1s[0]);
            Xunit.Assert.Equal(1, num1s[1]);
            Xunit.Assert.Equal(1, num2s[0]);


            var removeNums = new System.Collections.Generic.List<int>();
            unsupplyn1Obs.Subscribe(removeNums.Add);
            unsupplyn2Obs.Subscribe(removeNums.Add);

            multipleNotices.Numbers1.Items.Remove(n1);
            multipleNotices.Numbers2.Items.Remove(n1);
            multipleNotices.Numbers1.Items.Remove(n1);

            var ar = new Regulus.Utility.AutoPowerRegulator(new Utility.PowerRegulator());
            while (removeNums.Count < 3)
            {
                ar.Operate();
                    }
            Xunit.Assert.Equal(1, removeNums[0]);
            Xunit.Assert.Equal(1, removeNums[1]);
            Xunit.Assert.Equal(1, removeNums[2]);
            

            env.Dispose();

        }

        [Xunit.Fact()]        
        public void TestSupplyAndUnsupply()
        {
            var multipleNotices = new MultipleNotices();

            var env = new TestEnv<Entry<IMultipleNotices>>(new Entry<IMultipleNotices>(multipleNotices));

            var n1 = new Regulus.Projects.TestProtocol.Common.Number(1);
            var n2 = new Regulus.Projects.TestProtocol.Common.Number(2);
            var n3 = new Regulus.Projects.TestProtocol.Common.Number(3);

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

            

            Xunit.Assert.Equal(1, num1s[0]);
            Xunit.Assert.Equal(2, num1s[1]);
            Xunit.Assert.Equal(2, num1s[2]);
            Xunit.Assert.Equal(3, num1s[3]);
            Xunit.Assert.Equal(2, num2s[0]);
            Xunit.Assert.Equal(3, num2s[1]);

            var removeNum1s = new System.Collections.Generic.List<int>();
            unsupplyn1Obs.Subscribe(removeNum1s.Add);
            unsupplyn2Obs.Subscribe(removeNum1s.Add);

            multipleNotices.Numbers1.Items.Remove(n2);
            multipleNotices.Numbers2.Items.Remove(n2);

            var ar = new Regulus.Utility.AutoPowerRegulator(new Utility.PowerRegulator());
            while (removeNum1s.Count < 2)
            {
                ar.Operate();
            }
            Xunit.Assert.Equal(2, removeNum1s[0]);
            Xunit.Assert.Equal(2, removeNum1s[1]);            

            env.Dispose();
        }
    }*/
}
