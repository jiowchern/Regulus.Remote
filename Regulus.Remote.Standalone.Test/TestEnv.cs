using Regulus.Projects.TestProtocol.Common;
using Regulus.Projects.TestProtocol.Common.Ghost;

namespace Regulus.Remote.Standalone.Test
{
    public class TestEnv<T> where T : Regulus.Remote.IBinderProvider, System.IDisposable     
    {
        readonly ThreadUpdater _AgentUpdater;
        readonly IService _Service;
        readonly Ghost.IAgent _Agent;
        public readonly INotifierQueryable Queryable;
        public readonly T Entry;

        public TestEnv(T entry)
        {
            
            Entry = entry;
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
            Entry.Dispose();
            _AgentUpdater.Stop();
            _Service.Leave(_Agent);
            _Service.Dispose();

        }
    }
    
}
