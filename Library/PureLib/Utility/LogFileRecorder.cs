using System;
using System.IO;

namespace Regulus.Utility
{
	public class LogFileRecorder
	{
		private readonly StreamWriter _Writer;

		private int _Line;

		public LogFileRecorder(string name)
		{
			var file = string.Format("{0}_{1}.log", name, DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
			_Writer = File.AppendText(file);

			AppDomain.CurrentDomain.UnhandledException += _FinalSave;
		}

		private void _FinalSave(object sender, UnhandledExceptionEventArgs e)
		{	        
			Record(e.ExceptionObject.ToString());
			Save();
		}

		public void Record(string message)
		{
			lock(_Writer)
			{
				var time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
				_Writer.WriteLine("[{2}\t{0}] : {1}", time, message, ++_Line);
				
			}
		}

		public void Save()
		{
			lock (_Writer)
				_Writer.Flush();
		}
	}
}
