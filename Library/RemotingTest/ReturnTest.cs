using System;
using System.Diagnostics;
using System.Threading;


using Microsoft.VisualStudio.TestTools.UnitTesting;


using Regulus.Framework;
using Regulus.Net45;
using Regulus.Remoting;
using Regulus.Remoting.Ghost.Native;
using Regulus.Utility;


using SpinWait = System.Threading.SpinWait;
using Task = System.Threading.Tasks.Task;

namespace RemotingTest
{
	[TestClass]
	public class RemotingTest
	{
		private static volatile bool _ConnectEnable;

		[TestMethod]
		public void ServerReconnectTest()
		{
			Singleton<Log>.Instance.RecordEvent += msg => { Debug.Write(msg); };
			var launcher = new Launcher();
			var server = new Server();
			var serverAppliction = new Regulus.Remoting.Soul.Native.Server(server, 12345);
			launcher.Push(serverAppliction);
			var agent = Agent.Create();
			agent.BreakEvent += () => { RemotingTest._ConnectEnable = false; };

			launcher.Launch();
			Thread.Sleep(1000);
			RemotingTest._ConnectEnable = true;
			var task = new Task(RemotingTest._UpdateAgent(agent));
			task.Start();
			if(agent.Connect("127.0.0.1", 12345).WaitResult())
			{
				launcher.Shutdown();
			}
			else
			{
				throw new Exception("connect fail. 1");
			}

			task.Wait();
			Thread.Sleep(1000);
			launcher.Launch();
			Thread.Sleep(1000);
			RemotingTest._ConnectEnable = true;
			var task2 = new Task(RemotingTest._UpdateAgent(agent));
			task2.Start();
			if(agent.Connect("127.0.0.1", 12345).WaitResult())
			{
				launcher.Shutdown();
			}
			else
			{
				throw new Exception("connect fail. 2");
			}

			task2.Wait();
		}

		[TestMethod]
		public void TestUserMutiConnectDisconnect()
		{
			var launcher = new Launcher();
			var server = new Server();
			var serverAppliction = new Regulus.Remoting.Soul.Native.Server(server, 12345);
			launcher.Push(serverAppliction);
			launcher.Launch();

			var agent = Agent.Create();
			var user = new Regulus.Remoting.User(agent);
			var task = new Task(_UpdateUser(user));
			RemotingTest._ConnectEnable = true;
			task.Start();
			while(user.ConnectProvider.Ghosts.Length == 0)
			{
				;
			}

			var ghost = user.ConnectProvider.Ghosts[0];
			ghost.Connect("127.0.0.1", 12345);

			while(user.OnlineProvider.Ghosts.Length == 0)
			{
				;
			}

			agent.Disconnect();
			agent.Disconnect();

			var excep = false;
			try
			{
				ghost.Connect("127.0.0.1", 12345);
			}
			catch(SystemException se)
			{
				excep = se.Message == "Invalid Connect, to regain from the provider.";
			}

			RemotingTest._ConnectEnable = false;
			task.Wait();

			launcher.Shutdown();

			Assert.AreEqual(true, excep);
		}

		[TestMethod]
		public void TestUserReconnect()
		{
			var launcher = new Launcher();
			var server = new Server();
			var serverAppliction = new Regulus.Remoting.Soul.Native.Server(server, 12345);
			launcher.Push(serverAppliction);
			launcher.Launch();

			var agent = Agent.Create();
			var user = new Regulus.Remoting.User(agent);
			var task = new Task(_UpdateUser(user));
			RemotingTest._ConnectEnable = true;
			task.Start();

			Singleton<Log>.Instance.RecordEvent += Instance_RecordEvent;
			for(var i = 0; i < 10; ++i)
			{
				Debug.WriteLine("i " + i);
				while(user.ConnectProvider.Ghosts.Length == 0)
				{
					;
				}

				var connectResult = user.ConnectProvider.Ghosts[0].Connect("127.0.0.1", 12345).WaitResult();

				if(connectResult)
				{
					while(user.OnlineProvider.Ghosts.Length == 0)
					{
						;
					}

					user.OnlineProvider.Ghosts[0].Disconnect();
				}

				// System.Threading.Thread.Sleep(3000);
			}

			RemotingTest._ConnectEnable = false;
			task.Wait();
			launcher.Shutdown();
		}

		private void Instance_RecordEvent(string message)
		{
			Debug.WriteLine(message);
		}

		[TestMethod]
		public void UserTestInvalidConnect()
		{
			var user = new Regulus.Remoting.User(Agent.Create());
			var task = new Task(_UpdateUser(user));
			RemotingTest._ConnectEnable = true;
			task.Start();
			while(user.ConnectProvider.Ghosts.Length == 0)
			{
				;
			}

			var ghost = user.ConnectProvider.Ghosts[0];
			var result1 = ghost.Connect("127.0.0.1", 12345).WaitResult();

			var excep = false;
			try
			{
				ghost.Connect("127.0.0.1", 12345);
			}
			catch(SystemException se)
			{
				excep = se.Message == "Invalid Connect, to regain from the provider.";
			}

			RemotingTest._ConnectEnable = false;
			task.Wait();

			Assert.AreEqual(false, excep);
			Assert.AreEqual(false, result1);
		}

		[TestMethod]
		public void ConnectFailTest()
		{
			for(var i = 0; i < 3; ++i)
			{
				var agent = Agent.Create();
				RemotingTest._ConnectEnable = true;
				var task = new Task(RemotingTest._UpdateAgent(agent));
				task.Start();
				var connectResult = agent.Connect("127.0.0.1", 12345).WaitResult();
				agent.Disconnect();
				RemotingTest._ConnectEnable = false;
				task.Wait();

				Assert.AreEqual(false, connectResult);
			}
		}

		[TestMethod]
		public void ConnectTest()
		{
			var launcher = new Launcher();
			var server = new Server();
			var serverAppliction = new Regulus.Remoting.Soul.Native.Server(server, 12345);
			launcher.Push(serverAppliction);
			launcher.Launch();

			var agent = Agent.Create();

			RemotingTest._ConnectEnable = true;
			var task = new Task(RemotingTest._UpdateAgent(agent));
			task.Start();
			if(agent.Connect("127.0.0.1", 12345).WaitResult())
			{
				while(agent.QueryNotifier<ITestGPI>().Ghosts.Length == 0)
				{
					;
				}

				var result = agent.QueryNotifier<ITestGPI>().Ghosts[0].Add(1, 2).WaitResult();

				agent.Disconnect();
			}

			RemotingTest._ConnectEnable = false;
			task.Wait();

			RemotingTest._ConnectEnable = true;
			task = new Task(RemotingTest._UpdateAgent(agent));
			task.Start();
			if(agent.Connect("127.0.0.1", 12345).WaitResult())
			{
				while(agent.QueryNotifier<ITestGPI>().Ghosts.Length == 0)
				{
					;
				}

				var result = agent.QueryNotifier<ITestGPI>().Ghosts[0].Add(1, 2).WaitResult();

				agent.Disconnect();
			}

			RemotingTest._ConnectEnable = false;
			task.Wait();

			RemotingTest._ConnectEnable = true;
			task = new Task(RemotingTest._UpdateAgent(agent));
			task.Start();
			if(agent.Connect("127.0.0.1", 12345).WaitResult())
			{
				while(agent.QueryNotifier<ITestGPI>().Ghosts.Length == 0)
				{
					;
				}

				var result = agent.QueryNotifier<ITestGPI>().Ghosts[0].Add(1, 2).WaitResult();

				agent.Disconnect();
			}

			RemotingTest._ConnectEnable = false;
			task.Wait();

			RemotingTest._ConnectEnable = true;
			task = new Task(RemotingTest._UpdateAgent(agent));
			task.Start();
			if(agent.Connect("127.0.0.1", 12345).WaitResult())
			{
				while(agent.QueryNotifier<ITestGPI>().Ghosts.Length == 0)
				{
					;
				}

				var result = agent.QueryNotifier<ITestGPI>().Ghosts[0].Add(1, 2).WaitResult();

				agent.Disconnect();
			}

			RemotingTest._ConnectEnable = false;
			task.Wait();

			RemotingTest._ConnectEnable = true;
			task = new Task(RemotingTest._UpdateAgent(agent));
			task.Start();
			if(agent.Connect("127.0.0.1", 12345).WaitResult())
			{
				while(agent.QueryNotifier<ITestGPI>().Ghosts.Length == 0)
				{
					;
				}

				var result = agent.QueryNotifier<ITestGPI>().Ghosts[0].Add(1, 2).WaitResult();

				agent.Disconnect();
			}

			RemotingTest._ConnectEnable = false;
			task.Wait();

			launcher.Shutdown();
		}

		private static Action _UpdateAgent(IAgent agent)
		{
			return () =>
			{
				var sw = new SpinWait();

				agent.Launch();
				while(RemotingTest._ConnectEnable || agent.Connected)
				{
					agent.Update();

					sw.SpinOnce();
				}

				agent.Shutdown();
			};
		}

		private Action _UpdateUser(IUpdatable user)
		{
			return () =>
			{
				var sw = new SpinWait();

				user.Launch();
				while(RemotingTest._ConnectEnable)
				{
					user.Update();

					sw.SpinOnce();
				}

				user.Shutdown();
			};
		}

		[TestMethod]
		public void RemotingReturnTest()
		{
			var sw = new SpinWait();
			var launcher = new Launcher();
			var server = new Server();
			var serverAppliction = new Regulus.Remoting.Soul.Native.Server(server, 12345);
			var empty = new EmptyInputView();
			var app = new Client<IUser>(empty, empty);
			app.Selector.AddFactoty("1", new RemotingProvider());
			var userProvider = app.Selector.CreateUserProvider("1");
			var user = userProvider.Spawn("1");

			ITestReturn testReturn = null;

			user.TestReturnProvider.Return += test =>
			{
				testReturn = test;
				Assert.AreEqual(1, user.TestReturnProvider.Returns.Length);
			};
			user.Remoting.ConnectProvider.Supply += ConnectProvider_Supply;

			var updater = new Updater();
			updater.Add(app);
			launcher.Push(serverAppliction);
			launcher.Launch();
			var enable = true;
			var result2 = 0;
			while(enable)
			{
				updater.Working();
				sw.SpinOnce();
				if(testReturn != null)
				{
					testReturn.Test(1, 2).OnValue += r =>
					{
						r.Add(2, 3).OnValue += r2 => { result2 = r2; };
						enable = false;
					};

					enable = false;
					testReturn = null;
					GC.Collect();
				}
			}

			updater.Working();

			while(result2 == 0)
			{
				updater.Working();
			}

			GC.Collect();

			Assert.AreEqual(0, user.TestReturnProvider.Returns.Length);
			Assert.AreEqual(-3, result2);

			launcher.Shutdown();

			user.Remoting.ConnectProvider.Supply -= ConnectProvider_Supply;
			updater.Shutdown();
		}

		[TestMethod]
		public void StandalongReturnTest()
		{
			var server = new Server();

			var empty = new EmptyInputView();
			var app = new Client<IUser>(empty, empty);
			app.Selector.AddFactoty("1", new StandalongProvider(server));
			var userProvider = app.Selector.CreateUserProvider("1");
			var user = userProvider.Spawn("1");

			ITestReturn testReturn = null;
			user.Remoting.ConnectProvider.Supply += ConnectProvider_Supply;
			user.TestReturnProvider.Return += test =>
			{
				testReturn = test;
				Assert.AreEqual(1, user.TestReturnProvider.Returns.Length);
			};

			var updater = new Updater();
			updater.Add(app);
			updater.Add(server);
			var enable = true;
			var result2 = 0;

			while(enable)
			{
				updater.Working();

				if(testReturn != null)
				{
					testReturn.Test(1, 2).OnValue += r =>
					{
						r.Add(2, 3).OnValue += r2 => { result2 = r2; };
						enable = false;
					};

					testReturn = null;
				}
			}

			updater.Working();

			while(result2 == 0)
			{
				updater.Working();
			}

			GC.Collect();

			Assert.AreEqual(0, user.TestReturnProvider.Returns.Length);
			Assert.AreEqual(-3, result2);

			user.Remoting.ConnectProvider.Supply -= ConnectProvider_Supply;
			updater.Shutdown();
		}

		private void ConnectProvider_Supply(IConnect obj)
		{
			obj.Connect("127.0.0.1", 12345);
		}

		[TestMethod]
		public void TestUnsupportedBindChecker()
		{
			// var bindChecker = new Regulus.Remoting.BindGuard();
			// bindChecker.Check(typeof(IUnsupportedMethodIntefaceParam));            
		}
	}
}
