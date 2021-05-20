
namespace Regulus.Remote.Tests
{
    using Regulus.Remote.Reactive;
    
    using System.Reactive.Linq;
    public class ValueTests
    {
        [Xunit.Fact]
        public async System.Threading.Tasks.Task ConstructorOnValueTest()
        {
            var val = new Regulus.Remote.Value<int>(1);
            var vObs = from v in val.RemoteValue()
                            select v;
            var result = await vObs.FirstAsync();
            Xunit.Assert.Equal(1, result);

        }
        [Xunit.Fact]
        public async System.Threading.Tasks.Task SetOnValueTest()
        {
            var val = new Regulus.Remote.Value<int>();
            var vObs = from v in val.RemoteValue()
                       select v;

            val.SetValue(1);
            var result = await vObs.FirstAsync();
            Xunit.Assert.Equal(1, result);

        }

        [Xunit.Fact]
        public async System.Threading.Tasks.Task ConstructorAwaitOnValueTest()
        {
            var val = await new Regulus.Remote.Value<int>(1);

            Xunit.Assert.Equal(1, val);
        }

        [Xunit.Fact]
        public async System.Threading.Tasks.Task SetAwaitOnValueTest()
        {
            var val = new Regulus.Remote.Value<int>();
            val.SetValue(1);
            
            Xunit.Assert.Equal(1, await val);
        }
    }
}
