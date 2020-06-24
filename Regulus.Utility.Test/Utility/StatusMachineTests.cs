using NUnit.Framework;
using NSubstitute;

namespace Regulus.Utility.Tests
{


    [TestFixture()]
    public class StatusMachineTests
    {
        [Test()]
        public void PushOne()
        {

            var status = NSubstitute.Substitute.For<IStatus>();
            var machine = new Regulus.Utility.StatusMachine();
            machine.Push(status);
            machine.Update();
            machine.Termination();

            status.Received(1).Enter();
            status.Received(1).Update();
            status.Received(1).Leave();

        }

        [Test()]
        public void Change()
        {

            var status1 = NSubstitute.Substitute.For<IStatus>();
            var status2 = NSubstitute.Substitute.For<IStatus>();
            var machine = new Regulus.Utility.StatusMachine();
            machine.Push(status1);
            machine.Update();
            machine.Push(status2);
            machine.Update();
            machine.Termination();

            status1.Received(1).Enter();
            status1.Received(1).Update();
            status1.Received(1).Leave();

            status2.Received(1).Enter();
            status2.Received(1).Update();
            status2.Received(1).Leave();

        }
    }
}