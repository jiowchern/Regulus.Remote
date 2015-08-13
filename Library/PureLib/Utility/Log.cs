using System;

namespace Regulus.Utility
{
	public sealed class Log : Singleton<Log>
	{
		public delegate void RecordCallback(string message);

		public event RecordCallback RecordEvent
		{
			add { _AsyncRecord += value; }
			remove { _AsyncRecord -= value; }
		}

		private readonly AsyncExecuter _Executer;

		private RecordCallback _AsyncRecord;

		public Log()
		{
			_AsyncRecord = _EmptyRecord;
			_Executer = new AsyncExecuter();
		}

		private void _EmptyRecord(string message)
		{
		}

		private void _Write(string message)
		{
			_Executer.Push(new LogWritter(message, _AsyncRecord).Write);
		}

		public void WriteInfo(string message)
		{
			_Write(string.Format("[Info]{0}", message));
		}

		public void WriteDebug(string message)
		{
			_Write(string.Format("[Debug]{0}\r\n{1}", message, Environment.StackTrace));
		}
	}
}
