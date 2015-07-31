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
using System.IO;
using System.Threading;

using Regulus.Collection;
using Regulus.Utility;

using Console = Regulus.Utility.Console;
using SpinWait = System.Threading.SpinWait;

#endregion

namespace FormulaUserBot
{
	internal class Log : Singleton<Log>
	{
		private readonly string _File;

		private readonly Queue<string> _Messages;

		private bool _Enable;

		private Console.IViewer _View;

		public Log()
		{
			_File = string.Format("log_{0}.log", DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));

			_Messages = new Queue<string>();
			_Enable = true;
			ThreadPool.QueueUserWorkItem(this._Loging);
		}

		private void _Loging(object state)
		{
			var writer = File.AppendText(_File);

			var sw = new SpinWait();
			while (_Enable)
			{
				var messages = _Messages.DequeueAll();
				foreach (var msg in messages)
				{
					writer.WriteLine(msg);
				}

				sw.SpinOnce();
			}

			writer.Close();
		}

		internal void Initial(Console.IViewer view)
		{
			_View = view;
		}

		internal void WriteLine(string message)
		{
			_Messages.Enqueue(message);
		}

		internal void Final()
		{
			_Enable = false;
		}
	}
}