using System;
using System.Collections.Generic;
using System.Linq;


using Regulus.Framework;

namespace Regulus.Utility
{
	public class ConsoleInput : Console.IInput, IUpdatable
	{
		private event Console.OnOutput _OutputEvent;

		private readonly Doskey _Doskey;

		private readonly Stack<char> _InputData = new Stack<char>();

		private readonly string _Prompt;

		private readonly Console.IViewer _Viewer;

        readonly  ConsoleKeyReader _KeyReader;

        public ConsoleInput(Console.IViewer viewer)
		{
			_Viewer = viewer;
			_Doskey = new Doskey(10);

			_Prompt = ">>";
            _KeyReader = new ConsoleKeyReader();

        }

		event Console.OnOutput Console.IInput.OutputEvent
		{
			add { _OutputEvent += value; }
			remove { _OutputEvent -= value; }
		}

		bool IUpdatable.Update()
		{
			Update();
			return true;
		}

		void IBootable.Launch()
		{
            _KeyReader.Start();


        }



        void IBootable.Shutdown()
		{
            _KeyReader.Stop();

        }

		public void Update()
		{
			var cmd = _HandlerInput();
			if(cmd != null)
			{
				_OutputEvent(cmd);
				_Viewer.Write(_Prompt);
			}
		}

		protected string[] _HandlerInput()
		{

            ConsoleKeyInfo info;
            if (_KeyReader.Infos.TryDequeue( out info ))
            {
                return _HandlerInput(info, _InputData);
            }



            return null;
		}

		private string[] _HandlerInput(ConsoleKeyInfo key_info , Stack<char> chars)
		{
            
			var keyInfo = key_info;

			// Ignore if Alt or Ctrl is pressed.
			if((keyInfo.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt)
			{
				return null;
			}

			if((keyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
			{
				return null;
			}

			// Ignore if KeyChar value is \u0000.

			// Ignore tab key.
			if(keyInfo.Key == ConsoleKey.DownArrow)
			{
				var message = _Doskey.TryGetNext();
				if(message != null)
				{
					_ResetLine(chars, message);
				}
			}

			if(keyInfo.Key == ConsoleKey.UpArrow)
			{
				var message = _Doskey.TryGetPrev();

				if(message != null)
				{
					_ResetLine(chars, message);
				}
			}

			if(keyInfo.KeyChar == '\u0000')
			{
				return null;
			}

			if(keyInfo.Key == ConsoleKey.Tab)
			{
				return null;
			}

			if(keyInfo.Key == ConsoleKey.Escape)
			{
				return null;
			}

			if(keyInfo.Key == ConsoleKey.Backspace)
			{
				if(chars.Count() > 0)
				{
					chars.Pop();
					_Viewer.Write("\b \b");
				}
			}
			else if(keyInfo.Key == ConsoleKey.Enter)
			{
				var commands = new string(chars.Reverse().ToArray());
				commands = commands.Trim();
				if(commands.Length > 0)
				{
					chars.Clear();

					_Viewer.Write("\n");

					_Doskey.Record(commands);
					return commands.Split(
						new[]
						{
							' '
						}, 
						StringSplitOptions.RemoveEmptyEntries);
				}

				return null;
			}
			else
			{
				chars.Push(keyInfo.KeyChar);
				_Viewer.Write(keyInfo.KeyChar.ToString());
			}

			return null;
		}

		private void _ResetLine(Stack<char> chars, string message)
		{
			if(message != null)
			{
				foreach(var c in chars)
				{
					_Viewer.Write("\b");
				}

				foreach(var c in chars)
				{
					_Viewer.Write(" ");
				}

				_Viewer.Write("\r");
				_Viewer.Write(_Prompt + message);

				chars.Clear();

				foreach(var c in message)
				{
					chars.Push(c);
				}
			}
		}
	}
}
