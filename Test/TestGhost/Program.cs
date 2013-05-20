using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestGhost
{
	
	
	class Program
	{
		
		static void Main(string[] args)
		{

			TestRequester();
		}

		

		private static void TestRequester()
		{
			
			Samebest.Remoting.Ghost.Agent agent = new Samebest.Remoting.Ghost.Agent(new Samebest.Remoting.Ghost.Config() { Name = "Test" , Address = "127.0.0.1:5055"});

			agent.Launch(new Samebest.Remoting.Ghost.LinkState() { LinkSuccess = () => { }, LinkFail = () => { } });

			var provider = agent.QueryProvider<TestRemotingCommon.ITest>();

			provider.Supply += new Action<TestRemotingCommon.ITest>(provider_Supply);
			provider.Unsupply += new Action<TestRemotingCommon.ITest>(provider_Unsupply);
			
			while (agent.Update());

			agent.Shutdown();
		}

		static void provider_Unsupply(TestRemotingCommon.ITest obj)
		{
			 
		}

		static void provider_Supply(TestRemotingCommon.ITest obj)
		{
			Samebest.Remoting.Ghost.IGhost g = obj as Samebest.Remoting.Ghost.IGhost;
			var id =  g.GetID();
			
			obj.a1 += new Action(obj_a1);
			obj.a2 += new Action<int>(obj_a2);

			try
			{
				var val = obj.TestMethod(12345);
                val.OnValue += val_OnValue;
			}
			catch (System.AccessViolationException e)
			{
				
				throw;
			}

			
		}

        static void val_OnValue(int obj)
        {
            Console.WriteLine("on get value");
        }

		static void obj_a2(int obj)
		{
            Console.WriteLine("action 1");
		}

		static void obj_a1()
		{
            Console.WriteLine("action 2");
		}

		
	}
}

