using System;

namespace Regulus.Utility
{
    public class KeyReader
    {
        private readonly char _End;

        readonly  System.Collections.Generic.List<char> _Chars;
        
        public event System.Action<char[]> DoneEvent;
        public KeyReader(char end)
        {
            this._End = end;
            _Chars = new System.Collections.Generic.List<char>();
        }
        internal void Push(char c)
        {
            if (c != _End)
            {
                lock(_Chars)
                    _Chars.Add(c);
            }
            else
            {
                lock (_Chars)
                {
                    var chrs = _Chars.ToArray();
                    _Chars.Clear();
                    DoneEvent(chrs);
                }
                    
            }
        }
        internal void Push(System.Collections.Generic.IEnumerable<char> chars)
        {
            foreach(var c in chars)
            {
                Push(c);
            }
            
        }
    }
}