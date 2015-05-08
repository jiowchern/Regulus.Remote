using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Extension;
namespace RemotingTest
{
    [TestClass]
    public class ValueTest
    {        
        async void _TestAwait()
        {
            Regulus.Remoting.Value<bool> val = new Regulus.Remoting.Value<bool>();
            
            var task = val.ToTask();

            val.SetValue(true);
            var retValue = await task;
            Assert.AreEqual(true, retValue);
        }

        [TestMethod]
        public void TestAwait()
        {
            _TestAwait();
        }
    }
}
