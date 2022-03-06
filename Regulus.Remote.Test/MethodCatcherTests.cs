
using System;
using NUnit.Framework;
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
        [Test()]
        public void MethodCatcherTest1()
        {
            Utility.Reflection.TypeMethodCatcher catcher = new Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<Action<ITest>>)(ins => ins.Method()));
            Assert.AreEqual("Method", catcher.Method.Name);
        }

        [Test()]
        public void MethodCatcherTest2()
        {
            Utility.Reflection.TypeMethodCatcher catcher = new Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<Action<ITest>>)(ins => ins.Method2()));
            Assert.AreEqual("Method2", catcher.Method.Name);
        }

        [Test()]
        public void MethodCatcherTest3()
        {
            Utility.Reflection.TypeMethodCatcher catcher = new Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<Action<ITest, int, int>>)((ins, _1, _2) => ins.Method3(_1, _2)));
            Assert.AreEqual("Method3", catcher.Method.Name);
        }

    }
}