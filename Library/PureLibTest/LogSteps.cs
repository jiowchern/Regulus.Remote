using Microsoft.VisualStudio.TestTools.UnitTesting;


using Regulus.Utility;


using TechTalk.SpecFlow;

namespace RegulusLibraryTest
{
	[Binding]
	[Scope(Feature = "Log")]
	public class LogSteps
	{
		private readonly Log _Log;

		private volatile bool _GetData;

		private string _Message;

		private string _OutMessage;

		public LogSteps()
		{
			_Log = new Log();
			_Log.RecordEvent += _Log_RecordEvent;
		}

		private void _Log_RecordEvent(string message)
		{
			_OutMessage = message;
			_GetData = true;
		}

		[Given(@"Log寫入資料是""(.*)""")]
		public void 假設Log寫入資料是(string p0)
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

		[Then(@"頭(.*)個字元是""(.*)""")]
		public void 那麼頭個字元是(int p0, string p1)
		{
			while(_GetData == false)
			{
				;
			}

			var chars1 = _OutMessage.ToCharArray();
			var chars2 = p1.ToCharArray();

			for(var i = 0; i < chars2.Length; ++i)
			{
				Assert.AreEqual(chars1[i], chars2[i]);
			}
		}

		[Then(@"輸出為""(.*)""")]
		[Timeout(10000)]
		public void 那麼輸出為(string p0)
		{
			while(_GetData == false)
			{
				;
			}

			Assert.AreEqual(p0, _OutMessage);
		}
	}
}
