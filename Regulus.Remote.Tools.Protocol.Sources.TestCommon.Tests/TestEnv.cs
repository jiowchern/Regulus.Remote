using Regulus.Remote.Standalone;
using System.Linq;

namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon.Tests
{
    public class TestEnv<T,T2> where T : Regulus.Remote.IEntry, System.IDisposable
    {
        readonly ThreadUpdater _AgentUpdater;
        readonly Service _Service;
        readonly Ghost.IAgent _Agent;
        public readonly INotifierQueryable Queryable;
        public readonly T Entry;
        
        public TestEnv(T entry)
        {

            Entry = entry;
            IProtocol protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(typeof(T2).Assembly).Single();
            var ser=  new Regulus.Remote.Serializer(protocol.SerializeTypes);
            var internalSer = new Regulus.Remote.InternalSerializer();
            
            _Service = Regulus.Remote.Standalone.Provider.CreateService(entry,protocol, ser , Memorys.PoolProvider.Shared );
            
            _Agent = _Service.Create();
            
            Queryable = _Agent;
            
           
            _AgentUpdater = new ThreadUpdater(_Update);
            _AgentUpdater.Start();
        }

        private void _Update()
        {
            _Agent.Update();
        }

        public void Dispose()
        {
            
            Entry.Dispose();
            _AgentUpdater.Stop();
            _Service.Destroy(_Agent);
            _Service.Dispose();

        }
    }
}