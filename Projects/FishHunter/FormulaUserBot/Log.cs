using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormulaUserBot
{
    class Log : Regulus.Utility.Singleton<Log>
    {
        Regulus.Utility.Console.IViewer _View;
        string _File;
        public Log()
        {
            _File = string.Format("log_{0}.log", System.DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));            
            
        }
        internal void SetView(Regulus.Utility.Console.IViewer view)
        {
            _View = view;
        
        }

        ~Log()
        {
         
        }

        internal void WriteLine(string message)
        {
            //if (_View != null)
              //  _View.WriteLine(message);

            _Write(message);
        }
        async void _Write(string message)
        {
            using (System.IO.StreamWriter writer = System.IO.File.AppendText(_File))
            {
                await writer.WriteLineAsync(message);                
            }
        }

    }
}
