using Regulus.Utility.Reflection;
using System;

namespace Regulus.Utility.Test
{

    class Test
    {
        public void Method1()
        { }
    }
    public class ReflectionTest
    {
        [Xunit.Fact]
        public void GetMethod()
        {
            TypeMethodCatcher catcher = new TypeMethodCatcher((System.Linq.Expressions.Expression<Action<Test>>)(ins => ins.Method1()));
            Xunit.Assert.Equal("Method1", catcher.Method.Name);
        }
    }
}
