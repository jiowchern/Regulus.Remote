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

        
        public void Write(string message)
        {

            _Executer.Push(new LogWritter(message, _AsyncRecord).Write);            
        }
    }
}
