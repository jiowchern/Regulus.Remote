using NUnit.Framework;

namespace RegulusLibraryTest
{
    public class KeyReaderTest
    {


        [Test]
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

            Assert.AreEqual("ab", message);
        }

        [Test]
        public void TestMuti()
        {
            string message = "";
            Regulus.Utility.KeyReader reader = new Regulus.Utility.KeyReader('\r');
            reader.DoneEvent += (chars) =>
            {
                message = new string(chars);
            };
            reader.Push(new char[] { 'a', 'b', '\r' });



            Assert.AreEqual("ab", message);
        }

    }
}
