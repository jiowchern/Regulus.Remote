using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{

    class Doskey
    {
        int _Current;
        int _Capacity;

        List<string> _Records;        
        

        public Doskey(int capacity)
        {
            _Capacity = capacity;
            _Records = new List<string>();
        }
        private string _Get(int current)
        {
            if (current < _Records.Count)
                return _Records[current];
            return null;
        }
        int _Next()
        {
            if (++_Current > _Records.Count)
            {
                _Current = 0;
            }
            return _Current;
        }

        private int _Prev()
        {

            if (--_Current < 0)
            {
                _Current = _Records.Count;
            }
            return _Current;
        }

        internal string GetPrev()
        {
            return _Get(_Prev());
        }

        internal string GetNext()
        {
            return _Get(_Next());
        }

        

        internal void Record(string commands)
        {
            _Current = 0;
            _Records = _Records.Union(new string[] {commands}).ToList() ;
            if(_Records.Count > _Capacity)
            {
                _Records.RemoveAt(_Records.Count - 1);
            }
        }
    }
    public class ConsoleInput : Console.IInput , Regulus.Utility.IUpdatable
    {
        Doskey _Doskey;
        Stack<char> _InputData = new Stack<char>();
        Console.IViewer _Viewer;
        public ConsoleInput(Console.IViewer viewer)
        {
            _Viewer = viewer;
            _Doskey = new Doskey(10);
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
            

            // Ignore tab key.
            if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                string message = _Doskey.GetNext();
                _ResetLine(chars, message);                
            }
            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                string message = _Doskey.GetPrev();
                _ResetLine(chars, message);                
            }
            if (keyInfo.KeyChar == '\u0000')
                return null;

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
                _Viewer.Write(message);                

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
