using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Extension;
namespace RemotingTest
{
    [TestClass]
    public class RemotingTest
    {

        volatile static bool _ConnectEnable;
        [TestMethod , Timeout(5000)]
        public void ConnectTest()
        {

            Regulus.Utility.Launcher launcher = new Regulus.Utility.Launcher();
            Server server = new Server();
            var serverAppliction = new Regulus.Remoting.Soul.Native.Server(server, 12345);
            launcher.Push(serverAppliction);
            launcher.Launch();
            
            var agent = Regulus.Remoting.Ghost.Native.Agent.Create();

            _ConnectEnable = true;
            System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(NewMethod(  agent));
            task.Start();
            if(agent.Connect("127.0.0.1", 12345).WaitResult())
            {
                while (agent.QueryNotifier<ITestGPI>().Ghosts.Length == 0) ;
                int result = agent.QueryNotifier<ITestGPI>().Ghosts[0].Add(1, 2).WaitResult();

                agent.Disconnect();                
            }
            _ConnectEnable = false;
            task.Wait();


            _ConnectEnable = true; 
            task = new System.Threading.Tasks.Task(NewMethod(  agent));
            task.Start();                      
            if (agent.Connect("127.0.0.1", 12345).WaitResult())
            {
                while (agent.QueryNotifier<ITestGPI>().Ghosts.Length == 0) ;
                int result = agent.QueryNotifier<ITestGPI>().Ghosts[0].Add(1, 2).WaitResult();

                //agent.Disconnect();
                
            }
            _ConnectEnable = false;
            task.Wait();


            _ConnectEnable = true;
            task = new System.Threading.Tasks.Task(NewMethod(agent));
            task.Start();
            if (agent.Connect("127.0.0.1", 12345).WaitResult())
            {
                while (agent.QueryNotifier<ITestGPI>().Ghosts.Length == 0) ;
                int result = agent.QueryNotifier<ITestGPI>().Ghosts[0].Add(1, 2).WaitResult();

                agent.Disconnect();

            }
            _ConnectEnable = false;
            task.Wait();


            _ConnectEnable = true;
            task = new System.Threading.Tasks.Task(NewMethod(agent));
            task.Start();
            if (agent.Connect("127.0.0.1", 12345).WaitResult())
            {
                while (agent.QueryNotifier<ITestGPI>().Ghosts.Length == 0) ;
                int result = agent.QueryNotifier<ITestGPI>().Ghosts[0].Add(1, 2).WaitResult();

                //agent.Disconnect();

            }
            _ConnectEnable = false;
            task.Wait();


            _ConnectEnable = true;
            task = new System.Threading.Tasks.Task(NewMethod(agent));
            task.Start();
            if (agent.Connect("127.0.0.1", 12345).WaitResult())
            {
                while (agent.QueryNotifier<ITestGPI>().Ghosts.Length == 0) ;
                int result = agent.QueryNotifier<ITestGPI>().Ghosts[0].Add(1, 2).WaitResult();

                //agent.Disconnect();

            }
            _ConnectEnable = false;
            task.Wait();

            launcher.Shutdown();
        }

        private static Action NewMethod(Regulus.Remoting.IAgent agent)
        {
            return () =>
            {
               
                System.Threading.SpinWait sw = new System.Threading.SpinWait();
                
                agent.Launch();
                while (_ConnectEnable)
                {
                    agent.Update();
                   
                    sw.SpinOnce();
                }
              
                agent.Shutdown();
            };
        }


        [TestMethod]
        public void RemotingReturnTest()
        {
            System.Threading.SpinWait sw = new System.Threading.SpinWait();            
            Regulus.Utility.Launcher launcher = new Regulus.Utility.Launcher();
            Server server = new Server();
            Regulus.Remoting.Soul.Native.Server serverAppliction = new Regulus.Remoting.Soul.Native.Server(server, 12345);
            var empty = new EmptyInputView();
            var app = new Regulus.Framework.Client<IUser>(empty, empty);
            app.Selector.AddFactoty("1", new RemotingProvider());
            var userProvider = app.Selector.CreateUserProvider("1");
            var user = userProvider.Spawn("1");


            ITestReturn testReturn = null;
            user.Remoting.ConnectProvider.Supply += ConnectProvider_Supply;
            user.TestReturnProvider.Return += (test) =>
            {
                testReturn = test;                
                Assert.AreEqual(1, user.TestReturnProvider.Returns.Length);

            };

            Regulus.Utility.Updater updater = new Regulus.Utility.Updater();
            updater.Add(app);
            launcher.Push(serverAppliction);
            launcher.Launch();
            bool enable = true;
            int result2 = 0;
            while (enable)
            {
                updater.Working();
                sw.SpinOnce();
                if (testReturn != null)
                {

                    testReturn.Test(1, 2).OnValue += (r) =>
                    {
                        r.Add(2, 3).OnValue += (r2) => { result2 = r2; };
                        enable = false;

                    };

                    
                    enable = false;
                    testReturn = null;
                    System.GC.Collect();
                }
            }

            updater.Working();

            while (result2 == 0)
                updater.Working();

            System.GC.Collect();

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
            var app = new Regulus.Framework.Client<IUser>(empty, empty);
            app.Selector.AddFactoty("1", new StandalongProvider(server));
            var userProvider =  app.Selector.CreateUserProvider("1");
            var user = userProvider.Spawn("1");

            ITestReturn testReturn = null;
            user.Remoting.ConnectProvider.Supply += ConnectProvider_Supply;
            user.TestReturnProvider.Return += (test)=>
            {
                testReturn = test;
                Assert.AreEqual(1, user.TestReturnProvider.Returns.Length);

            };

            Regulus.Utility.Updater updater = new Regulus.Utility.Updater();
            updater.Add(app);
            updater.Add(server);
            bool enable = true;
            int result2 = 0;

            
            while (enable)
            {

                updater.Working();
                
                
                if(testReturn!= null)
                {
                    testReturn.Test(1, 2).OnValue += (r) =>
                    {
                        r.Add(2, 3).OnValue += (r2) => { result2 = r2  ; };                        
                        enable = false;
                        
                    };

                    testReturn = null;
                    
                }
            }

            updater.Working();
            
            while (result2 == 0)
                updater.Working();

            System.GC.Collect();

            
            
            Assert.AreEqual(0, user.TestReturnProvider.Returns.Length);
            Assert.AreEqual(-3, result2);

            user.Remoting.ConnectProvider.Supply -= ConnectProvider_Supply;
            updater.Shutdown();
        }

        

        void ConnectProvider_Supply(Regulus.Utility.IConnect obj)
        {
            obj.Connect("127.0.0.1", 12345);
        }


        [TestMethod]
        public void TestUnsupportedBindChecker()
        {
            //var bindChecker = new Regulus.Remoting.BindGuard();
            //bindChecker.Check(typeof(IUnsupportedMethodIntefaceParam));            
        }

        
    }



}
