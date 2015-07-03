using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus;

namespace Regulus.Utility
{
    
    
    sealed public class Log : Regulus.Utility.Singleton<Log>
    {
        
        public delegate void RecordCallback(string message);

        public event RecordCallback RecordEvent
        {
            add { _AsyncRecord += value; }
            remove { _AsyncRecord -= value; }
        }
        RecordCallback _AsyncRecord;
        

        Regulus.Utility.AsyncExecuter _Executer;
   

        public Log()
        {
            
            _AsyncRecord = _EmptyRecord;
            _Executer = new AsyncExecuter();
        }
        

        private void _EmptyRecord(string message)
        {
            
        }

        
        void _Write(string message)
        {
            _Executer.Push(new LogWritter(message, _AsyncRecord).Write);            
        }

        public void WriteInfo(string message)
        {
            
            _Write(string.Format("[Info]{0}", message));
        }

        public void WriteDebug(string message)
        {
            _Write(string.Format("[Debug]{0}\r\n{1}", message, System.Environment.StackTrace));
        }
    }
}
