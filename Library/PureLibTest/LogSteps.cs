using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TechTalk.SpecFlow;

namespace PureLibTest
{

   
    [Binding]
    public class LogSteps 
    {
        Regulus.Utility.Log _Log;
      
        string _Message;
        string _OutMessage;
       
        public LogSteps()
        {
            _Log = new Regulus.Utility.Log();
            _Log.RecordEvent += _Log_RecordEvent;
        }

        void _Log_RecordEvent(string message)
        {
            _OutMessage = message;
        }                
        [Given(@"資料是""(.*)""")]
        public void 假設資料是(string p0)
        {
            _Message = p0;
            
        }

        [When(@"寫入到LogInfo")]
        public void 當寫入到LogInfo()
        {
            _Log.WriteInfo(_Message);
        }

        [When(@"寫入到LogDebug")]
        public void 當寫入到LogDebug()
        {
            _Log.WriteDebug(_Message);
        }

        
        [Then(@"輸出為""(.*)""")]
        public void 那麼輸出為(string p0)
        {
            Assert.AreEqual(p0, _OutMessage);
        }
    }
}
