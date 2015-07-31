// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Log type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

#endregion

namespace Regulus.Utility
{
	public sealed class Log : Singleton<Log>
	{
		public event RecordCallback RecordEvent
		{
			add { this._AsyncRecord += value; }
			remove { this._AsyncRecord -= value; }
		}

		private readonly AsyncExecuter _Executer;

		private RecordCallback _AsyncRecord;

		public Log()
		{
			this._AsyncRecord = this._EmptyRecord;
			this._Executer = new AsyncExecuter();
		}

		public delegate void RecordCallback(string message);

		private void _EmptyRecord(string message)
		{
		}

		private void _Write(string message)
		{
			this._Executer.Push(new LogWritter(message, this._AsyncRecord).Write);
		}

		public void WriteInfo(string message)
		{
			this._Write(string.Format("[Info]{0}", message));
		}

		public void WriteDebug(string message)
		{
			this._Write(string.Format("[Debug]{0}\r\n{1}", message, Environment.StackTrace));
		}
	}
}