using Regulus.Remote.Ghost;
using System.Threading;
using System.Threading.Tasks;

namespace Regulus.Remote.Client.Tcp
{
    public class Connecter 
    {
        private readonly IAgent _Agent;

        IOnlineable _Online;
        public Connecter(IAgent agent)
        {
        
            this._Agent = agent;
        
        }
        public async Task<IOnlineable> Connect(System.Net.EndPoint endpoint)
        {      
            if(_Online != null)
            {
                _Online.Disconnect();
                _Online = null;
            }
            var connecter = new Regulus.Network.Tcp.Connecter();
            Regulus.Network.IConnectable connectable = connecter;            
            var result = await connectable.Connect(endpoint);
            if(!result)
            {
                return null;
            }
            _Online = new Online(connecter, _Agent);
            return _Online;

        }

        
    }
}