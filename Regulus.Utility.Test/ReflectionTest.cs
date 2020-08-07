using Regulus.Utility.Reflection;
using System;

namespace Regulus.Utility.Test
{

    class Test
    {
        public void Method1()
        { }
    }
    class ReflectionTest
    {
        [NUnit.Framework.Test()]
        public void GetMethod()
        {
            TypeMethodCatcher catcher = new TypeMethodCatcher((System.Linq.Expressions.Expression<Action<Test>>)(ins => ins.Method1()));
            NUnit.Framework.Assert.AreEqual("Method1", catcher.Method.Name);
        }
    }
}
