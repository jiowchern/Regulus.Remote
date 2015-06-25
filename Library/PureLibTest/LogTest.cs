using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
namespace PureLibTest
{

    
    [TestClass]
    public class LogTest
    {
       
        [TestMethod]
        public void TestWrite()
        {

            System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>();
            
            
            Regulus.Utility.Log.Instance.RecordEvent  += (message) => 
            {
                lock (messages)
                    messages.Add(message);                    
            };
            
            Regulus.Utility.Log.Instance.WriteInfo("123");
            Regulus.Utility.Log.Instance.WriteInfo("456");
            Regulus.Utility.Log.Instance.WriteInfo("789");
            Regulus.Utility.Log.Instance.WriteInfo("123");
            Regulus.Utility.Log.Instance.WriteInfo("1");
            

            while(true )
            {
                lock(messages)
                {
                    if (messages.Count >= 5)
                        break;
                }
                    
            }

            Assert.AreEqual("[Info]123" , messages[0]);
            Assert.AreEqual("[Info]456", messages[1]);
            Assert.AreEqual("[Info]789", messages[2]);
            Assert.AreEqual("[Info]123", messages[3]);
            Assert.AreEqual("[Info]1", messages[4]);
            

        }
    }
}
