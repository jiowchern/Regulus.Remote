
namespace Regulus.Remote.Tests
{
    using Regulus.Remote.Reactive;
    
    using System.Reactive.Linq;
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
