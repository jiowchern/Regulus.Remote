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
        public RecordCallback AsyncRecord { set ; private get; }

        Regulus.Utility.AsyncExecuter _Executer;
        
        
        

        public Log()
        {            
            
            AsyncRecord = _EmptyRecord;
            _Executer = new AsyncExecuter();
        }

        private void _EmptyRecord(string message)
        {
            
        }

        
        public void Write(string message)
        {

            _Executer.Push(new LogWritter(message, AsyncRecord).Write );            
        }
    }
}
