using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public interface ILogger 
    {
        void Log(string message);                        
    }

    public class ConsoleLogger : Samebest.Utility.Singleton<ConsoleLogger>, ILogger , IDisposable
    {
        System.IO.StreamWriter _StreamWriter;
        System.DateTime _Begin;
        public void Launch(string name)
        {

            _Begin = System.DateTime.Now;
            _StreamWriter = new System.IO.StreamWriter(name + "_" + string.Format("{0:yyyyMMddHHmmssffff}" , _Begin) + ".log");
            _StreamWriter.WriteLine("start log at : " + _Begin.ToString());
        }
        public void Log(string message)
        {
            if (_StreamWriter != null)
            {
                var current = System.DateTime.Now - _Begin;
                _StreamWriter.WriteLine(current.TotalSeconds.ToString() + "\t" + message);
            }
        }

        public void Shutdown()
        {
            _StreamWriter.Close();
        }

        void IDisposable.Dispose()
        {
            Shutdown();
        }
    }
}
