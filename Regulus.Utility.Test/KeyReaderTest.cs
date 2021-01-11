using Xunit;

namespace RegulusLibraryTest
{
    public class KeyReaderTest
    {


        [Xunit.Fact]
        public void TestSignle()
        {
            string message = "";
            Regulus.Utility.KeyReader reader = new Regulus.Utility.KeyReader('\r');
            reader.DoneEvent += (chars) =>
            {
                message = new string(chars);
            };
            reader.Push('a');
            reader.Push('b');
            reader.Push('\r');

            Assert.Equal("ab", message);
        }

        [Xunit.Fact]
        public void TestMuti()
        {
            string message = "";
            Regulus.Utility.KeyReader reader = new Regulus.Utility.KeyReader('\r');
            reader.DoneEvent += (chars) =>
            {
                message = new string(chars);
            };
            reader.Push(new char[] { 'a', 'b', '\r' });



            Assert.Equal("ab", message);
        }

    }
}
