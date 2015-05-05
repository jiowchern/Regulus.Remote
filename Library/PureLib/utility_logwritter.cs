using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus
{
    public class LogWritter
    {
        private string message;
        private Utility.Log.RecordCallback AsyncRecord;

        public LogWritter(string message, Utility.Log.RecordCallback AsyncRecord)
        {
            // TODO: Complete member initialization
            this.message = message;
            this.AsyncRecord = AsyncRecord;
        }

        internal void Write()
        {
            AsyncRecord(message);
        }
    }
}
