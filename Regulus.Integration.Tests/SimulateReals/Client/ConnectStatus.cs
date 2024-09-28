using Regulus.Remote.Client;
using Regulus.Utility;

namespace Regulus.Integration.Tests
{
    namespace SimulateReals
    {

        namespace Client
        {
            class ConnectStatus : Regulus.Utility.IStatus
            {
                private readonly int _Port;
                private readonly TcpConnectSet _Set;

                public event System.Action<TcpConnectSet> ConnectedEvent;
                public event System.Action ConnectFailEvent;
                bool? _Done;
                public ConnectStatus(int port , Regulus.Remote.IProtocol protocol )
                {
                    _Set = Regulus.Remote.Client.Provider.CreateTcpAgent(protocol);                    
                    this._Port = port;
                }
                public async void Enter()
                {
                    var result = await _Set.Connector.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, _Port))                        ;
                    _Done = result;                    
                }

                public void Leave()
                {
                    
                }

                void IStatus.Update()
                {
                    if(_Done.HasValue)
                    {
                        ConnectedEvent(_Set);
                    }
                    else
                    {
                        ConnectFailEvent();
                    }
                }
            }
        }
    }
    
}