using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{

    internal class Doskey
    {
        System.Collections.Generic.List<string> _Stack;
        int _Capacity;
        int _Current;
        public Doskey(int capacity)
        {
            _Capacity = capacity;
            _Stack = new List<string>(capacity);
        }


        internal void Record(string p)
        {
            
            _Stack.Add(p);
            
            if (_Stack.Count > _Capacity)
                _Stack.RemoveAt(0);

            _Current = _Stack.Count ;
        }

        internal string TryGetPrev()
        {
            if (_Stack.Count <= 0)
                return null;
            if(_Current - 1 >= 0 )
            {
                return _Stack[--_Current];
            }            
            return null;
        }
        internal string TryGetNext()
        {
            if (_Stack.Count <= 0)
                return null;

            if (_Current + 1 < _Stack.Count)
            {
                return _Stack[++_Current];
            }

            return null;
        }

       
        
    }
    public class ConsoleInput : Console.IInput , Regulus.Utility.IUpdatable
    {
        Doskey _Doskey;
        Stack<char> _InputData = new Stack<char>();
        Console.IViewer _Viewer;

        string _Prompt;
        public ConsoleInput(Console.IViewer viewer)
        {
            _Viewer = viewer;
            _Doskey = new Doskey(10);

            _Prompt = ">>";
        }

        public void Update()
        {
            string[] cmd = _HandlerInput() ;
            if (cmd != null)
            {
                _OutputEvent(cmd);
                _Viewer.Write(_Prompt);
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
            

            // Ignore tab key.
            if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                var message = _Doskey.TryGetNext();
                if (message != null)
                    _ResetLine(chars, message);                
            }
            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                var message = _Doskey.TryGetPrev();
                
                if (message != null)
                    _ResetLine(chars, message);                
            }
            if (keyInfo.KeyChar == '\u0000')
                return null;

            if (keyInfo.Key == ConsoleKey.Tab)
                return null;
            if (keyInfo.Key == ConsoleKey.Escape)
                return null;

            if (keyInfo.Key == ConsoleKey.Backspace )
            {
                if (chars.Count() > 0)
                {
                    chars.Pop();
                    _Viewer.Write("\b \b");                
                }
                
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {

                string commands = new string(chars.Reverse().ToArray());
                commands = commands.Trim();
                if (commands.Length > 0)
                {                    
                    chars.Clear();

                    _Viewer.Write("\n");

                    _Doskey.Record(commands);
                    return commands.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
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
            if (message != null)
            {
                foreach (var c in chars)
                    _Viewer.Write("\b");
                foreach (var c in chars)
                    _Viewer.Write(" ");
                _Viewer.Write("\r");
                _Viewer.Write(_Prompt + message);                

                chars.Clear();

                foreach(var c in message)
                {
                    chars.Push(c);
                }
                
            }            
        }

        event Console.OnOutput _OutputEvent;
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

        void Framework.IBootable.Launch()
        {
            
        }

        void Framework.IBootable.Shutdown()
        {
            
        }
    }
}
