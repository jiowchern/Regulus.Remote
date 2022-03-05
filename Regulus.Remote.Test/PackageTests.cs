namespace RemotingTest
{
    public class PackageTests
    {
        [NUnit.Framework.Test]
        public void WriterRestart()
        {
            var serializer = NSubstitute.Substitute.For<Regulus.Remote.IInternalSerializable>();
            var peer1 = NSubstitute.Substitute.For<Regulus.Network.IStreamable>();
            var peer2 = NSubstitute.Substitute.For<Regulus.Network.IStreamable>();
            var writer = new Regulus.Remote.PackageWriter<int>(serializer);
            writer.Start(peer1);
            writer.Stop();
            writer.Start(peer2);
            writer.Stop();

        }
    }

}
