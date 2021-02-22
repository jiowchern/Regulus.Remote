using Regulus.Projects.TestProtocol.Common;
using Regulus.Projects.TestProtocol.Common.Ghost;

namespace Regulus.Remote.Standalone.Test
{
    public class SampleTestEnv
    {
        readonly ThreadUpdater _AgentUpdater;
        readonly IService _Service;
        readonly Ghost.IAgent _Agent;
        public readonly INotifierQueryable Queryable;
        public readonly Sample Sample;
        public SampleTestEnv()
        {
            var entry = new SampleEntry();
            Sample = entry.Sample;
            IProtocol protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(typeof(CISample).Assembly);
            _Service = new Regulus.Remote.Standalone.Service(entry, protocol);
            _Agent = new Regulus.Remote.Ghost.Agent(protocol);

            _AgentUpdater = new ThreadUpdater(_Agent.Update);

            _Service.Join(_Agent);
            _AgentUpdater.Start();

            Queryable = _Agent;
        }
        

        public void Dispose()
        {
            Sample.Dispose();
            _AgentUpdater.Stop();
            _Service.Leave(_Agent);
            _Service.Dispose();

        }
    }
}
