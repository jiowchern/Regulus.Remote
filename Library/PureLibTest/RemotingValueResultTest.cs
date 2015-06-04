using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Remoting;
namespace PureLibTest
{
    [TestClass]
    public class RemotingValueResultTest
    {
        [TestMethod , Timeout(5000)]
        public void TestRemotingValueResult()
        {

            Regulus.Remoting.Value<bool> val = new Regulus.Remoting.Value<bool>();
            System.Timers.Timer timer = new System.Timers.Timer(1);
            timer.Start();
            timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => { val.SetValue(true); };

            val.Result();            
        }
       
    }
}
