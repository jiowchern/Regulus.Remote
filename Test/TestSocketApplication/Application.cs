using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Regulus.Remoting
{
    [ProtoBuf.ProtoContract]
    public class Package
    {
        [ProtoBuf.ProtoMember(1) ]
        public byte Code;
        [ProtoBuf.ProtoMember(2, DynamicType = true)]
        public Dictionary<byte, object> Args;
    }
}
namespace Regulus.Remoting.Soul
{
    public interface IUser : Regulus.Game.IFramework
    { 

    }

    
    public class NativeAppliaction : Regulus.Game.ConsoleFramework<IUser> , Regulus.Utility.IUpdatable
    {

        class Client : Regulus.Remoting.IRequestQueue, Regulus.Remoting.IResponseQueue
        {
            System.Net.Sockets.TcpClient _Client;
            Regulus.Remoting.Soul.SoulProvider _SoulProvider;
            System.Collections.Generic.Queue<Regulus.Remoting.Package> _Writes;
            Regulus.Game.StageMachine _ReadMachine;
            Regulus.Game.StageMachine _WriteMachine;

            public Client(System.Net.Sockets.TcpClient client)
            {                
                _Client = client;
                _SoulProvider = new Remoting.Soul.SoulProvider(this , this);
                _Writes = new Queue<Remoting.Package>();

                _ReadMachine = new Game.StageMachine();
                _WriteMachine = new Game.StageMachine();

                _ToWrite();
                _ToRead();
            }
            private void _ToWrite()
            {
                var stage = new NetworkStreamWriteStage(_Client.GetStream(), _Writes);
                stage.WriteCompletionEvent += _ToWrite;
                _WriteMachine.Push(stage);
            }
            private void _ToRead()
            {
                var stage = new NetworkStreamReadStage(_Client.GetStream(), _Client.ReceiveBufferSize);
                stage.ReadCompletionEvent += (buffer) =>
                {
                    using (var stream = new System.IO.MemoryStream(buffer))
                    {
                        var package = ProtoBuf.Serializer.Deserialize<Package>(stream);
                        _HandlePackage(package);
                    }
                    _ToRead();
                };
                _ReadMachine.Push(stage);
            }

            private void _HandlePackage(Package package)
            {
                if (package.Code == (byte)ClientToServerPhotonOpCode.Ping)
                {
                    (this as Regulus.Remoting.IResponseQueue).Push((int)ServerToClientPhotonOpCode.Ping, new Dictionary<byte, object>());
                }
            }

            public void Update()
            {
                _SoulProvider.Update();

                _ReadMachine.Update();
                _WriteMachine.Update();
            }

            

            void Remoting.IResponseQueue.Push(byte cmd, Dictionary<byte, object> args)
            {
                _Writes.Enqueue(new Regulus.Remoting.Package() { Code = cmd, Args = args });
            }

            event Action<Guid, string, Guid, object[]> Remoting.IRequestQueue.InvokeMethodEvent
            {
                add {  }
                remove {  }
            }

            event Action Remoting.IRequestQueue.BreakEvent
            {
                add {  }
                remove {  }
            }

            void IRequestQueue.Update()
            {
                
            }
        }
        class Controller : IController
        {
            string IController.Name
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            event OnSpawnUser IController.UserSpawnEvent
            {
                add { throw new NotImplementedException(); }
                remove { throw new NotImplementedException(); }
            }

            event OnSpawnUserFail IController.UserSpawnFailEvent
            {
                add { throw new NotImplementedException(); }
                remove { throw new NotImplementedException(); }
            }

            event OnUnspawnUser IController.UserUnpawnEvent
            {
                add { throw new NotImplementedException(); }
                remove { throw new NotImplementedException(); }
            }

            void IController.Look()
            {
                throw new NotImplementedException();
            }

            void IController.NotLook()
            {
                throw new NotImplementedException();
            }

            bool Game.IFramework.Update()
            {
                throw new NotImplementedException();
            }

            void Game.IFramework.Launch()
            {
                throw new NotImplementedException();
            }

            void Game.IFramework.Shutdown()
            {
                throw new NotImplementedException();
            }
        }

        System.Net.Sockets.TcpListener _Listener;
        private int _Port;
        public NativeAppliaction(int port ,Regulus.Utility.Console.IViewer view, Regulus.Utility.Console.IInput input)
            : base(view, input)
        {
            _Port = port;
            _NewClients = new System.Collections.Concurrent.ConcurrentQueue<Client>();
            _Clients = new List<Client>();
        }
        protected override Regulus.Game.ConsoleFramework<IUser>.ControllerProvider[] _ControllerProvider()
        {
            return new NativeAppliaction.ControllerProvider[] 
            {
                new NativeAppliaction.ControllerProvider { Command = "stand" , Spawn =  _BuildStandController},                
            };
        }

        private IController _BuildStandController()
        {
            return new Controller();
        }


        System.Collections.Concurrent.ConcurrentQueue<Client> _NewClients;
        System.Collections.Generic.List<Client> _Clients;
        
        bool Utility.IUpdatable.Update()
        {            
            _HandleClients(_Clients, _NewClients);
            return true;
        }

        private void _HandleClients(List<Client> clients, System.Collections.Concurrent.ConcurrentQueue<Client> new_clients)
        {
            if (new_clients.Count > 0)
            {
                Client client ;
                if (new_clients.TryDequeue(out client))
                {
                    clients.Add(client);
                }                
            }

            Parallel.ForEach<Client>(clients, c => 
            {
                c.Update();                
            });
        }


        private void _HandleConnect(System.Net.Sockets.TcpListener listener, System.Collections.Concurrent.ConcurrentQueue<Client> clients)
        {
            listener.BeginAcceptTcpClient(_OnAcceptTcpClient, null);
        }

        private void _OnAcceptTcpClient(IAsyncResult ar)
        {
            var client = _Listener.EndAcceptTcpClient(ar);
            _NewClients.Enqueue(new Client(client));

            _Listener.BeginAcceptTcpClient(_OnAcceptTcpClient, null);
        }

        void Framework.ILaunched.Launch()
        {
            _Listener = System.Net.Sockets.TcpListener.Create(_Port);
            _Listener.Start();
            _HandleConnect(_Listener, _NewClients);
        }

        void Framework.ILaunched.Shutdown()
        {
            _Listener.Stop();
        }
    }
}
