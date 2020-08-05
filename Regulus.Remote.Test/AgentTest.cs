
using Regulus.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Remote.Tests
{
    public interface IGpi
    {
        void Method();
    }
    
    public class AgentTest
    {
        [NUnit.Framework.Test()]
        public void StandaloneConnectEventTest()
        {
            // todo
            throw new NotImplementedException();
            /*var protocol = NSubstitute.Substitute.For<IProtocol>();
            bool has = false;
            var agent = new Regulus.Remote.Standalone.Agent(protocol) as IAgent;
            agent.ConnectEvent += () =>
            {
                has = true;
            };

            agent.QueryNotifier<IConnect>().Supply += (connecter) => {
                connecter.Connect(new System.Net.IPEndPoint(0, 0));
            };
            agent.Launch();
            agent.Update();
            agent.Shutdown();
            NUnit.Framework.Assert.True(has);*/
        }
    }
}