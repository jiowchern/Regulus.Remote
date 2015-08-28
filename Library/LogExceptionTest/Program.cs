using System;


using Regulus.Utility;


using Console = System.Console;

namespace LogExceptionTest
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var recorder = new LogFileRecorder("LogTest");
			Singleton<Log>.Instance.RecordEvent += recorder.Record;

		    while(true)
		    {
		        
                System.Threading.Tasks.Parallel.For(
		            1,
		            100,
		            (i) =>
		            {
                        Singleton<Log>.Instance.WriteDebug(System.DateTime.Now.ToString());
                        if(System.Console.KeyAvailable)
                            throw new Exception("aaa");
                    });
                
            }

            
		}

		
	}
}
