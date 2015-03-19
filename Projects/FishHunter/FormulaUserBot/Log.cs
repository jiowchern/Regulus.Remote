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
        

        Regulus.Collection.Queue<string> _Messages;
        bool _Enable ;
        public Log()
        {
            _File = string.Format("log_{0}.log", System.DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));

            
            _Messages = new Regulus.Collection.Queue<string>();
            _Enable = true;
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(_Loging));    
        }

        private void _Loging(object state)
        {
            System.IO.StreamWriter writer = System.IO.File.AppendText(_File);
            
            System.Threading.SpinWait sw = new System.Threading.SpinWait();
            while (_Enable)
            {
                if (_Messages.Count > 0)
                {
                    writer.WriteLine(_Messages.Dequeue());                    
                }
                else
                    sw.SpinOnce();
            }

            writer.Close();
            
        }
        internal void Initial(Regulus.Utility.Console.IViewer view)
        {
            _View = view;
            
        }


        internal void WriteLine(string message)
        {            
            _Messages.Enqueue(message);
            
                
        }



        internal void Final()
        {
            _Enable = false;
        }
    }
}
