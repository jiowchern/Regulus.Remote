using Regulus.Remote;
using Regulus.Remote.Client;
using System;

namespace Regulus.Integration.Tests
{
    namespace SimulateReals
    {

        namespace Client
        {
            class User
            {
                
                private readonly Regulus.Utility.StatusMachine _Machine;
                private readonly int port;
                private readonly IProtocol protocol;
                public readonly int Id;

                int _Count;

                public User(int id,int port , Regulus.Remote.IProtocol protocol)
                {
                    Id = id;
                    _Machine = new Utility.StatusMachine();

                    this.port = port;
                    this.protocol = protocol;

                    _ToConnect();
                    
                }

                private void _ToConnect()
                {
                    var status = new ConnectStatus(port , protocol);
                    status.ConnectFailEvent += _ToConnect;
                    status.ConnectedEvent += _ToStay;

                    _Machine.Push(status);  
                }

                private void _ToStay(TcpConnectSet set)
                {
                    var status = new StayStatus(1, set.Agent);
                    status.DoneEvent += ()=> { _ToDisconnect(set.Connector); };
                    _Machine.Push(status);
                }

                private void _ToDisconnect(Regulus.Network.Tcp.Connector connector)
                {
                    var status = new DisconnectStatus(connector);
                    status.DoneEvent += _ToConnect;
                    _Machine.Push(status);
                    _Count++;
                }

                public void Update()
                {
                    _Machine.Update();
                }

                public bool IsDone()
                {
                    return _Count >= 10;
                }
            }
        }
    }
    
}