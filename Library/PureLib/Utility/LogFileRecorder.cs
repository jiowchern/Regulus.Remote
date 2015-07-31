// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogFileRecorder.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the LogFileRecorder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.IO;

#endregion

namespace Regulus.Utility
{
	public class LogFileRecorder
	{
		private readonly StreamWriter _Writer;

		private int _Line;

		public LogFileRecorder(string name)
		{
			var file = string.Format("{0}_{1}.log", name, DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
			this._Writer = File.AppendText(file);
		}

		public void Record(string message)
		{
			lock (this._Writer)
			{
				var time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
				this._Writer.WriteLine("[{2}\t{0}] : {1}", time, message, ++this._Line);
			}
		}

		public void Save()
		{
			this._Writer.Flush();
		}
	}
}