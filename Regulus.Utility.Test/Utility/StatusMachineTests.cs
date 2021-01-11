using NSubstitute;
using Xunit;

namespace Regulus.Utility.Tests
{


    
    public class StatusMachineTests
    {
        [Xunit.Fact]
        public void PushOne()
        {

            IStatus status = NSubstitute.Substitute.For<IStatus>();
            StatusMachine machine = new Regulus.Utility.StatusMachine();
            machine.Push(status);
            machine.Update();
            machine.Termination();

            status.Received(1).Enter();
            status.Received(1).Update();
            status.Received(1).Leave();

        }

        [Xunit.Fact]
        public void Change()
        {

            IStatus status1 = NSubstitute.Substitute.For<IStatus>();
            IStatus status2 = NSubstitute.Substitute.For<IStatus>();
            StatusMachine machine = new Regulus.Utility.StatusMachine();
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