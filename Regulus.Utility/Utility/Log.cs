using System;
using System.Linq.Expressions;

namespace Regulus.Utility
{
    public sealed class Log : Singleton<Log>
    {
        public delegate void RecordCallback(string message);

        public event RecordCallback RecordEvent
        {
            add { _AsyncRecord += value; }
            remove { _AsyncRecord -= value; }
        }

        private readonly AsyncExecuter _Executer;

        private RecordCallback _AsyncRecord;

        public Log()
        {
            _AsyncRecord = _EmptyRecord;
            _Executer = new AsyncExecuter();
        }

        private void _EmptyRecord(string message)
        {
        }


        public void WriteInfo(Expression<Func<string>> message)
        {
            _Executer.Push(new LogWritter(LogWritter.TYPE.INFO, message, _AsyncRecord).Write);
        }

        public void WriteInfo(string message)
        {
            _Executer.Push(new LogWritter(LogWritter.TYPE.INFO, () => message, _AsyncRecord).Write);
        }

        public void WriteDebug(Expression<Func<string>> message)
        {
            _Executer.Push(new LogWritter(LogWritter.TYPE.DEBUG, message, _AsyncRecord).Write);
        }
        public void WriteDebug(string message)
        {
            _Executer.Push(new LogWritter(LogWritter.TYPE.DEBUG, () => message, _AsyncRecord).Write);
        }

        public void Shutdown()
        {
            _Executer.Shutdown();
        }
    }
}
