
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
        //Guid Id { get; }
        //Regulus.Remote.Value<bool> MethodReturn();

        //event Action<float, string> OnCallEvent;
    }
    public class AgentTest
    {
        [NUnit.Framework.Test()]
        public void StandaloneConnectEventTest()
        {
            var protocol = NSubstitute.Substitute.For<IProtocol>();
            bool has = false;
            var agent = new Regulus.Remote.Standalone.Agent(protocol) as IAgent;
            agent.ConnectEvent += () =>
            {
                has = true;
            };

            agent.Connect(new System.Net.IPEndPoint(0, 0));

            NUnit.Framework.Assert.True(has);
        }



    }
}