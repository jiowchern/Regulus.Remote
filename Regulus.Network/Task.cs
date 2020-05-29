using System;

namespace Regulus.Network
{
    public class Task
    {
        public byte[] Buffer;
        public int Offset;
        public int Count;

        private int _DoneCount;
        private bool _Done;

        public Task()
        {
            this.Buffer = new byte[0];
            _Done = false;
        }
        public void Done(int count)
        {
            _Done = true;
            _DoneCount = count;
            if (_DoneEvent != null)
                _DoneEvent(_DoneCount);
        }

        internal Action<int> _DoneEvent;
        public event Action<int> DoneEvent
        {
            add
            {
                _DoneEvent += value;
                if (_Done)
                    value(_DoneCount);
            }
            remove { _DoneEvent -= value; }
        }
    }
}