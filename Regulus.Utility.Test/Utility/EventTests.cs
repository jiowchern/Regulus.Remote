using Xunit;

namespace Regulus.Utility.Tests
{

    public class TestInvoker : Regulus.Utility.Invoker
    {
        public new void Invoke()
        {
            base.Invoke();
        }
    }

    
    public class EventTests
    {


        [Xunit.Fact]
        public void InvokeTest1()
        {
            TestInvoker testEvent = new TestInvoker();
            Regulus.Utility.Notifier notifier = testEvent;
            bool ok = false;
            notifier.Subscribe += () => ok = true;
            testEvent.Invoke();

            Assert.True(ok);
        }

        [Xunit.Fact]
        public void InvokeTest2()
        {
            TestInvoker testEvent = new TestInvoker();
            Regulus.Utility.Notifier notifier = testEvent;
            bool ok = false;
            testEvent.Invoke();
            notifier.Subscribe += () => ok = true;


            Assert.True(ok);
        }
    }
}