
namespace Regulus.Remote.Tests
{
    using Regulus.Remote.Reactive;
    
    using System.Reactive.Linq;

    public class PingTests
    {
        [NUnit.Framework.Test]
        public void OnePingTest()
        {
            var ping = new Regulus.Remote.Ping(1f);
            int count = 0;
            ping.TriggerEvent += () =>
            {
                count++;
            };

            System.Threading.Thread.Sleep(500);
            ping.GetSeconds();
            NUnit.Framework.Assert.AreEqual(0, count);


            System.Threading.Thread.Sleep(1000);
            ping.GetSeconds();
            NUnit.Framework.Assert.AreEqual(1, count);
            
            ping.Update();

            System.Threading.Thread.Sleep(500);
            ping.GetSeconds();
            NUnit.Framework.Assert.AreEqual(1, count);

            ping.Update();

            System.Threading.Thread.Sleep(3000);
            ping.GetSeconds();
            NUnit.Framework.Assert.AreEqual(2, count);

        }

        [NUnit.Framework.Test]
        public void TimeTest()
        {
            var ping = new Regulus.Remote.Ping(1f);
            System.Threading.Thread.Sleep(1000);            
            ping.Update();
            var sec1 = ping.GetSeconds();
            NUnit.Framework.Assert.LessOrEqual(1f , sec1);
            ping.Update();
            var sec2 = ping.GetSeconds();
            NUnit.Framework.Assert.LessOrEqual(sec2 , 1f );



        }
    }
    public class ValueTests
    {
        
        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task ConstructorOnValueTest()
        {
            var val = new Regulus.Remote.Value<int>(1);
            var vObs = from v in val.RemoteValue()
                            select v;
            var result = await vObs.FirstAsync();
            NUnit.Framework.Assert.AreEqual(1, result);

        }
        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task SetOnValueTest()
        {
            var val = new Regulus.Remote.Value<int>();
            var vObs = from v in val.RemoteValue()
                       select v;

            val.SetValue(1);
            var result = await vObs.FirstAsync();
            NUnit.Framework.Assert.AreEqual(1, result);

        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task ConstructorAwaitOnValueTest()
        {
            var val = await new Regulus.Remote.Value<int>(1);

            NUnit.Framework.Assert.AreEqual(1, val);
        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task SetAwaitOnValueTest()
        {
            var val = new Regulus.Remote.Value<int>();
            val.SetValue(1);
            
            NUnit.Framework.Assert.AreEqual(1, await val);
        }
    }
}
