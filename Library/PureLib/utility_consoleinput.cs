using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class ConsoleInput : Console.IInput
    {
        
        Stack<char> _InputData = new Stack<char>();
        Console.IViewer _Viewer;
        public ConsoleInput(Console.IViewer viewer)
        {
            _Viewer = viewer;
        }

        public void Update()
        {
            string[] cmd = _HandlerInput() ;
            if (cmd != null)
            {
                _OutputEvent(cmd);
            }
        }


        protected string[] _HandlerInput()
        {
            if (System.Console.KeyAvailable)
            {
                return _HandlerInput(_InputData);

            }
            return null;
        }

        private string[] _HandlerInput(Stack<char> chars)
        {
            var keyInfo = System.Console.ReadKey(true);
            // Ignore if Alt or Ctrl is pressed.
            if ((keyInfo.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt)
                return null;
            if ((keyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                return null;
            // Ignore if KeyChar value is \u0000.
            if (keyInfo.KeyChar == '\u0000')
                return null;
            // Ignore tab key.
            if (keyInfo.Key == ConsoleKey.Tab)
                return null;
            if (keyInfo.Key == ConsoleKey.Escape)
                return null;

            if (keyInfo.Key == ConsoleKey.Backspace && chars.Count() > 0)
            {
                chars.Pop();
                _Viewer.Write("\b \b");
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {

                string commands = new string(chars.Reverse().ToArray());
                Regulus.Utility.Singleton<Regulus.Utility.ConsoleLogger>.Instance.Log("Enter Command : " + commands);
                chars.Clear();

                _Viewer.Write("\n");
                return commands.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                chars.Push(keyInfo.KeyChar);
                _Viewer.Write(keyInfo.KeyChar.ToString());
            }
            return null;
        }

        event Console.OnOutput _OutputEvent;
        event Console.OnOutput Console.IInput.OutputEvent
        {
            add { _OutputEvent += value; }
            remove { _OutputEvent -= value; }
        }

        
    }
}
