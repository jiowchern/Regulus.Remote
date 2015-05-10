using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            Regulus.Utility.Log.Instance.Write("123");
            Regulus.Utility.Log.Instance.Write("456");
            Regulus.Utility.Log.Instance.Write("789");
            Regulus.Utility.Log.Instance.Write("123");
            Regulus.Utility.Log.Instance.Write("1");


            while(true )
            {
                lock(messages)
                {
                    if (messages.Count >= 5)
                        break;
                }
                    
            }

            Assert.AreEqual("123" , messages[0]);
            Assert.AreEqual("456", messages[1]);
            Assert.AreEqual("789", messages[2]);
            Assert.AreEqual("123", messages[3]);
            Assert.AreEqual("1", messages[4]);
        }
    }
}
