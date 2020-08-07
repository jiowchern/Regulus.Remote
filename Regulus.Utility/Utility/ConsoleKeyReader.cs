using System;

namespace Regulus.Utility
{
    internal class ConsoleKeyReader
    {
        private readonly System.Threading.Tasks.Task _Task;
        private volatile bool _Enable;

        public readonly System.Collections.Concurrent.ConcurrentQueue<ConsoleKeyInfo> Infos;
        public ConsoleKeyReader()
        {
            _Task = new System.Threading.Tasks.Task(_Handle);
            Infos = new System.Collections.Concurrent.ConcurrentQueue<ConsoleKeyInfo>();
        }

        private void _Handle()
        {
            while (_Enable)
            {
                ConsoleKeyInfo info = System.Console.ReadKey(true);
                Infos.Enqueue(info);
            }
        }

        internal void Start()
        {
            _Enable = true;
            _Task.Start();

        }



        internal void Stop()
        {
            _Enable = false;

        }
    }
}