using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RemotingTest
{
    [TestClass]
    public class ReturnTest
    {
        [TestMethod]
        public void StandalongTest()
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
            while (enable)
            {
                updater.Update();

                if(testReturn!= null)
                {
                    enable = false;
                    testReturn = null;
                    System.GC.Collect();
                    System.Threading.Thread.Sleep(3);
                }
            }

            updater.Update();

            Assert.AreEqual(0, user.TestReturnProvider.Returns.Length);
            


            
            user.Remoting.ConnectProvider.Supply -= ConnectProvider_Supply;
            updater.Shutdown();
        }

        

        void ConnectProvider_Supply(Regulus.Utility.IConnect obj)
        {
            obj.Connect("127.0.0.1", 12345);
        }
    }
}
