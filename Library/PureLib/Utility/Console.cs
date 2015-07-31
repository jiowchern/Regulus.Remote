// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Console.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Console type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;

#endregion

namespace Regulus.Utility
{
	public class Console
	{
		[Flags]
		public enum LogFilter
		{
			None = 0, 

			RegisterCommand = 1, 

			All = LogFilter.RegisterCommand
		}

		private readonly Dictionary<string, string> _Commands;

		private LogFilter _Filter;

		private IInput _Input;

		private IViewer _Viewer;

		public Command Command { get; private set; }

		public Console(IInput input, IViewer viewer)
		{
			this.Command = new Command();
			this._Filter = LogFilter.All;
			this._Commands = new Dictionary<string, string>();
			this._Initial(input, viewer);
		}

		public delegate void OnOutput(string[] command_paraments);

		public interface IInput
		{
			event OnOutput OutputEvent;
		}

		public interface IViewer
		{
			void WriteLine(string message);

			void Write(string message);
		}

		~Console()
		{
			this._Release();
		}

		private void _Initial(IInput input, IViewer viewer)
		{
			this._Viewer = viewer;
			this._Input = input;
			this._Input.OutputEvent += this._Run;

			this.Command.RegisterEvent += this._OnRegister;
			this.Command.UnregisterEvent += this._OnUnregister;

			this.Command.Register("help", this._Help);
		}

		private void _Help()
		{
			foreach (var cmd in this._Commands)
			{
				this._Viewer.WriteLine(string.Format("{0}\t[{1}]", cmd.Key, cmd.Value));
			}
		}

		public void SetLogFilter(LogFilter flag)
		{
			this._Filter = flag;
		}

		private void _OnUnregister(string command)
		{
			if ((this._Filter & LogFilter.RegisterCommand) == LogFilter.RegisterCommand)
			{
				this._Viewer.WriteLine("Remove Command > " + command);
			}

			this._Commands.Remove(command);
		}

		private void _Release()
		{
			this.Command.Unregister("help");
			this.Command.UnregisterEvent -= this._OnUnregister;
			this.Command.RegisterEvent -= this._OnRegister;
			this._Input.OutputEvent -= this._Run;
		}

		private void _OnRegister(string command, Command.CommandParameter ret, Command.CommandParameter[] args)
		{
			var argString = string.Empty;
			foreach (var arg in args)
			{
				argString += (string.IsNullOrEmpty(arg.Description)
					? string.Empty
					: arg.Description + ":") + arg.Param.Name + " ";
			}

			if ((this._Filter & LogFilter.RegisterCommand) == LogFilter.RegisterCommand)
			{
				this._Viewer.WriteLine("Add Command > " + command + " " + argString);
			}

			this._Commands.Add(command, argString);
		}

		private void _Run(string[] command_paraments)
		{
			var cmdArgs = new Queue<string>(command_paraments);
			if (cmdArgs.Count > 0)
			{
				var cmd = cmdArgs.Dequeue();

				try
				{
					var runCount = this.Command.Run(cmd, cmdArgs.ToArray());
					if (runCount != 0)
					{
						this._Viewer.WriteLine("Done.");
					}
					else
					{
						this._Viewer.WriteLine(string.Format("Invalid command. {0}", cmd));
					}
				}
				catch (ArgumentException argument_exception)
				{
					this._Viewer.WriteLine("Parameter error: " + argument_exception.Message);
				}
			}
		}
	}
}