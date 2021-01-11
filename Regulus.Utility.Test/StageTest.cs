namespace RegulusLibraryTest
{
    using NSubstitute;
    public class StageTest
    {
        [Xunit.Fact]
        public void MachineTest()
        {

            Regulus.Utility.IBootable stage1 = NSubstitute.Substitute.For<Regulus.Utility.IBootable>();
            Regulus.Utility.IBootable stage2 = NSubstitute.Substitute.For<Regulus.Utility.IBootable>();
            Regulus.Utility.StageMachine machine = new Regulus.Utility.StageMachine();

            machine.Push(stage1);
            machine.Push(stage2);

            machine.Clean();

            stage1.Received().Launch();
            stage1.Received().Shutdown();
            stage2.Received().Launch();
            stage2.Received().Shutdown();

        }
    }


}
