using System;
using System.Collections.Generic;

namespace Regulus.Utility
{
	public class Console
	{
		public delegate void OnOutput(string[] command_paraments);

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
			Command = new Command();
			_Filter = LogFilter.All;
			_Commands = new Dictionary<string, string>();
			_Initial(input, viewer);
		}

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
			_Release();
		}

		private void _Initial(IInput input, IViewer viewer)
		{
			_Viewer = viewer;
			_Input = input;
			_Input.OutputEvent += _Run;

			Command.RegisterEvent += _OnRegister;
			Command.UnregisterEvent += _OnUnregister;

			Command.Register("help", _Help);
		}

		private void _Help()
		{
			foreach(var cmd in _Commands)
			{
				_Viewer.WriteLine(string.Format("{0}\t[{1}]", cmd.Key, cmd.Value));
			}
		}

		public void SetLogFilter(LogFilter flag)
		{
			_Filter = flag;
		}

		private void _OnUnregister(string command)
		{
			if((_Filter & LogFilter.RegisterCommand) == LogFilter.RegisterCommand)
			{
				_Viewer.WriteLine("Remove Command > " + command);
			}

			_Commands.Remove(command);
		}

		private void _Release()
		{
			Command.Unregister("help");
			Command.UnregisterEvent -= _OnUnregister;
			Command.RegisterEvent -= _OnRegister;
			_Input.OutputEvent -= _Run;
		}

		private void _OnRegister(string command, Command.CommandParameter ret, Command.CommandParameter[] args)
		{
			var argString = string.Empty;
			foreach(var arg in args)
			{
				argString += (string.IsNullOrEmpty(arg.Description)
					              ? string.Empty
					              : arg.Description + ":") + arg.Param.Name + " ";
			}

			if((_Filter & LogFilter.RegisterCommand) == LogFilter.RegisterCommand)
			{
				_Viewer.WriteLine("Add Command > " + command + " " + argString);
			}

			_Commands.Add(command, argString);
		}

		private void _Run(string[] command_paraments)
		{
			var cmdArgs = new Queue<string>(command_paraments);
			if(cmdArgs.Count > 0)
			{
				var cmd = cmdArgs.Dequeue();

				try
				{
					var runCount = Command.Run(cmd, cmdArgs.ToArray());
					if(runCount != 0)
					{
						_Viewer.WriteLine("Done.");
					}
					else
					{
						_Viewer.WriteLine(string.Format("Invalid command. {0}", cmd));
					}
				}
				catch(ArgumentException argument_exception)
				{
					_Viewer.WriteLine("Parameter error: " + argument_exception.Message);
				}
			}
		}
	}
}
