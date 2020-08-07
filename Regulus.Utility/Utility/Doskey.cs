using System.Collections.Generic;

namespace Regulus.Utility
{
    public class Doskey
    {
        private readonly int _Capacity;

        private readonly List<string> _Stack;

        private int _Current;

        public Doskey(int capacity)
        {
            _Capacity = capacity;
            _Stack = new List<string>(capacity);
        }

        public void Record(string p)
        {
            _Stack.Add(p);

            if (_Stack.Count > _Capacity)
            {
                _Stack.RemoveAt(0);
            }

            _Current = _Stack.Count;
        }

        public string GetPrev()
        {
            string str = TryGetPrev();
            return str == null ? "" : str;
        }

        public string TryGetPrev()
        {
            if (_Stack.Count <= 0)
            {
                return null;
            }

            if (_Current - 1 >= 0)
            {
                return _Stack[--_Current];
            }

            return null;
        }

        public string GetNext()
        {
            string str = TryGetNext();
            return str == null ? "" : str;
        }

        public string TryGetNext()
        {
            if (_Stack.Count <= 0)
            {
                return null;
            }

            if (_Current + 1 < _Stack.Count)
            {
                return _Stack[++_Current];
            }

            return null;
        }
    }
}
