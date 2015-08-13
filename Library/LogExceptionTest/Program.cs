using System;


using Regulus.Utility;


using Console = System.Console;

namespace LogExceptionTest
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var recorder = new LogFileRecorder("LogTest");
			Singleton<Log>.Instance.RecordEvent += recorder.Record;

			throw new Exception("aaa");
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine(e.ToString());
		}

		private static void Instance_RecordEvent(string message)
		{
			Console.WriteLine(message);
		}
	}
}
