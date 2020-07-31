namespace RegulusLibraryTest
{
	using NSubstitute;
    public class StageTest
	{
		[NUnit.Framework.Test]
		public void MachineTest()
		{
			
			var stage1 = NSubstitute.Substitute.For<Regulus.Utiliey.IBootable>();
			var stage2 = NSubstitute.Substitute.For<Regulus.Utiliey.IBootable>();
			var machine = new Regulus.Utility.StageMachine();

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
