
using System;
using Xunit;
namespace Regulus.Remote.AOT.Tests
{
    interface ITest
    {
        void Method();
        int Method2();

        int Method3(int a, int b);

        event Action Event1;
    }
    
    public class MethodCatcherTests
    {
        [Fact()]
        public void MethodCatcherTest1()
        {
            Utility.Reflection.TypeMethodCatcher catcher = new Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<Action<ITest>>)(ins => ins.Method()));
            Assert.Equal("Method", catcher.Method.Name);
        }

        [Fact()]
        public void MethodCatcherTest2()
        {
            Utility.Reflection.TypeMethodCatcher catcher = new Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<Action<ITest>>)(ins => ins.Method2()));
            Assert.Equal("Method2", catcher.Method.Name);
        }

        [Fact()]
        public void MethodCatcherTest3()
        {
            Utility.Reflection.TypeMethodCatcher catcher = new Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<Action<ITest, int, int>>)((ins, _1, _2) => ins.Method3(_1, _2)));
            Assert.Equal("Method3", catcher.Method.Name);
        }

    }
}