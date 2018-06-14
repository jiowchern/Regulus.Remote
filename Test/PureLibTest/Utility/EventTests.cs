using NUnit.Framework;
using Regulus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Utility.Tests
{

    public class TestInvoker : Regulus.Utility.Invoker
    {
        public new void Invoke()
        {
            base.Invoke();
        }
    }
    [TestFixture()]
    public class EventTests
    {


        [Test()]
        public void InvokeTest1()
        {
            var testEvent = new TestInvoker();
            Regulus.Utility.Notifier notifier = testEvent;
            bool ok = false;
            notifier.Subscribe += () => ok = true;
            testEvent.Invoke();

            Assert.AreEqual(true , ok);
        }

        [Test()]
        public void InvokeTest2()
        {
            var testEvent = new TestInvoker();
            Regulus.Utility.Notifier notifier = testEvent;
            bool ok = false;
            testEvent.Invoke();
            notifier.Subscribe += () => ok = true;
            

            Assert.AreEqual(true, ok);
        }
    }
}