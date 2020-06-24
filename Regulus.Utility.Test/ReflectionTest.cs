using Regulus.Utility.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

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
            var catcher = new TypeMethodCatcher((System.Linq.Expressions.Expression<Action<Test>>)(ins => ins.Method1()));
            NUnit.Framework.Assert.AreEqual("Method1", catcher.Method.Name);
        }
    }
}
