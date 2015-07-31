// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleInput.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ConsoleInput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.Framework;

#endregion

namespace Regulus.Utility
{
	public class ConsoleInput : Console.IInput, IUpdatable
	{
		private event Console.OnOutput _OutputEvent;

		private readonly Doskey _Doskey;

		private readonly Stack<char> _InputData = new Stack<char>();

		private readonly string _Prompt;

		private readonly Console.IViewer _Viewer;

		public ConsoleInput(Console.IViewer viewer)
		{
			this._Viewer = viewer;
			this._Doskey = new Doskey(10);

			this._Prompt = ">>";
		}

		event Console.OnOutput Console.IInput.OutputEvent
		{
			add { this._OutputEvent += value; }
			remove { this._OutputEvent -= value; }
		}

		bool IUpdatable.Update()
		{
			this.Update();
			return true;
		}

		void IBootable.Launch()
		{
		}

		void IBootable.Shutdown()
		{
		}

		public void Update()
		{
			var cmd = this._HandlerInput();
			if (cmd != null)
			{
				this._OutputEvent(cmd);
				this._Viewer.Write(this._Prompt);
			}
		}

		protected string[] _HandlerInput()
		{
			if (System.Console.KeyAvailable)
			{
				return this._HandlerInput(this._InputData);
			}

			return null;
		}

		private string[] _HandlerInput(Stack<char> chars)
		{
			var keyInfo = System.Console.ReadKey(true);

			// Ignore if Alt or Ctrl is pressed.
			if ((keyInfo.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt)
			{
				return null;
			}

			if ((keyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
			{
				return null;
			}

			// Ignore if KeyChar value is \u0000.

			// Ignore tab key.
			if (keyInfo.Key == ConsoleKey.DownArrow)
			{
				var message = this._Doskey.TryGetNext();
				if (message != null)
				{
					this._ResetLine(chars, message);
				}
			}

			if (keyInfo.Key == ConsoleKey.UpArrow)
			{
				var message = this._Doskey.TryGetPrev();

				if (message != null)
				{
					this._ResetLine(chars, message);
				}
			}

			if (keyInfo.KeyChar == '\u0000')
			{
				return null;
			}

			if (keyInfo.Key == ConsoleKey.Tab)
			{
				return null;
			}

			if (keyInfo.Key == ConsoleKey.Escape)
			{
				return null;
			}

			if (keyInfo.Key == ConsoleKey.Backspace)
			{
				if (chars.Count() > 0)
				{
					chars.Pop();
					this._Viewer.Write("\b \b");
				}
			}
			else if (keyInfo.Key == ConsoleKey.Enter)
			{
				var commands = new string(chars.Reverse().ToArray());
				commands = commands.Trim();
				if (commands.Length > 0)
				{
					chars.Clear();

					this._Viewer.Write("\n");

					this._Doskey.Record(commands);
					return commands.Split(new[]
					{
						' '
					}, StringSplitOptions.RemoveEmptyEntries);
				}

				return null;
			}
			else
			{
				chars.Push(keyInfo.KeyChar);
				this._Viewer.Write(keyInfo.KeyChar.ToString());
			}

			return null;
		}

		private void _ResetLine(Stack<char> chars, string message)
		{
			if (message != null)
			{
				foreach (var c in chars)
				{
					this._Viewer.Write("\b");
				}

				foreach (var c in chars)
				{
					this._Viewer.Write(" ");
				}

				this._Viewer.Write("\r");
				this._Viewer.Write(this._Prompt + message);

				chars.Clear();

				foreach (var c in message)
				{
					chars.Push(c);
				}
			}
		}
	}
}