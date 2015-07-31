// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogSteps.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the LogSteps type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Regulus.Utility;

using TechTalk.SpecFlow;

#endregion

namespace PureLibraryTest
{
	[Binding]
	public class LogSteps
	{
		private readonly Log _Log;

		private volatile bool _GetData;

		private string _Message;

		private string _OutMessage;

		public LogSteps()
		{
			this._Log = new Log();
			this._Log.RecordEvent += this._Log_RecordEvent;
		}

		private void _Log_RecordEvent(string message)
		{
			this._OutMessage = message;
			this._GetData = true;
		}

		[Given(@"Log寫入資料是""(.*)""")]
		public void 假設Log寫入資料是(string p0)
		{
			this._Message = p0;
		}

		[When(@"寫入到LogInfo")]
		public void 當寫入到LogInfo()
		{
			this._Log.WriteInfo(this._Message);
		}

		[When(@"寫入到LogDebug")]
		public void 當寫入到LogDebug()
		{
			this._Log.WriteDebug(this._Message);
		}

		[Then(@"頭(.*)個字元是""(.*)""")]
		public void 那麼頭個字元是(int p0, string p1)
		{
			while (this._GetData == false)
			{
				;
			}

			var chars1 = this._OutMessage.ToCharArray();
			var chars2 = p1.ToCharArray();

			for (var i = 0; i < chars2.Length; ++i)
			{
				Assert.AreEqual(chars1[i], chars2[i]);
			}
		}

		[Then(@"輸出為""(.*)""")]
		[Timeout(10000)]
		public void 那麼輸出為(string p0)
		{
			while (this._GetData == false)
			{
				;
			}

			Assert.AreEqual(p0, this._OutMessage);
		}
	}
}