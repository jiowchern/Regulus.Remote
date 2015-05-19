using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class LogFileRecorder
    {        
        System.IO.StreamWriter _Writer;
        int _Line;
        public LogFileRecorder(string name)
        {
            var file = string.Format("{0}_{1}.log",name, System.DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
            _Writer = System.IO.File.AppendText(file);

            AppDomain.CurrentDomain.UnhandledException += _FinalError;
        }

        private void _FinalError(object sender, UnhandledExceptionEventArgs e)
        {
            Record(e.ToString());
        }

        public void Record(string message)
        {
            lock(_Writer)
            {
                var time = System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                _Writer.WriteLine(string.Format("[{2}\t{0}] : {1}", time, message, ++_Line));
                _Writer.Flush();
            }                        
        }

        public void Save()
        {
            _Writer.Flush();
        }
    }
}
