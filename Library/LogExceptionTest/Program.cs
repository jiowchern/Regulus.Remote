using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogExceptionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Regulus.Utility.LogFileRecorder recorder = new Regulus.Utility.LogFileRecorder("LogTest");
            Regulus.Utility.Log.Instance.RecordEvent += recorder.Record;
            

            throw new System.Exception("aaa");
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Console.WriteLine(e.ToString());
        }

        static void Instance_RecordEvent(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}
