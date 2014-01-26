using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Regulus.Remoting
{
    public class NetworkStreamWriteStage : Regulus.Game.IStage
    {
        System.Net.Sockets.NetworkStream _Stream;
        Queue<Package> _Packages;
        public NetworkStreamWriteStage(System.Net.Sockets.NetworkStream stream, Queue<Package> packages)
        {
            _Stream = stream;
            _Packages = packages;
        }
        void Game.IStage.Enter()
        {
            if (_Packages.Count > 0)
            {
                var package = _Packages.Dequeue();
                var stream = new MemoryStream();
                ProtoBuf.Serializer.Serialize<Package>(stream, package);
                var buffer = stream.ToArray();
                _Stream.BeginWrite(buffer, 0, buffer.Length, _WriteCompletion, null);
            }
            else
            {
                WriteCompletionEvent();
            }
        }

        public delegate void OnWriteCompletion();
        public event OnWriteCompletion WriteCompletionEvent;
        private void _WriteCompletion(IAsyncResult ar)
        {
            _Stream.EndWrite(ar);
            WriteCompletionEvent();
        }

        void Game.IStage.Leave()
        {

        }

        void Game.IStage.Update()
        {

        }
    }
    public class NetworkStreamReadStage : Regulus.Game.IStage
    {
        System.Net.Sockets.NetworkStream _Stream;
        byte[] _Buffer;
        public NetworkStreamReadStage(System.Net.Sockets.NetworkStream stream, int size)
        {
            _Buffer = new byte[size];
            _Stream = stream;
        }
        void Game.IStage.Enter()
        {
            _Stream.BeginRead(_Buffer, 0, _Buffer.Length, _Readed, null);
        }

        public delegate void OnReadCompletion(byte[] buffer);
        public event OnReadCompletion ReadCompletionEvent;

        private void _Readed(IAsyncResult ar)
        {
            _Stream.EndRead(ar);
            ReadCompletionEvent(_Buffer);
        }

        void Game.IStage.Leave()
        {

        }

        void Game.IStage.Update()
        {

        }
    }
}
namespace Regulus.Remoting.Ghost.Native
{
    
    public class Agent : Regulus.Utility.IUpdatable , Regulus.Remoting.IGhostRequest
    {
        Regulus.Remoting.AgentCore _Core;
        System.Net.Sockets.TcpClient _Tcp;
        Queue<Package> _WaitWiters;
        Regulus.Game.StageMachine _ReadMachine;
        Regulus.Game.StageMachine _WriteMachine;
        string _IpAddress;
        int _Port;
        public Agent(string ipaddress,int port)
        {
            _WaitWiters = new Queue<Package>();
            _IpAddress = ipaddress;
            _Port = port;
            _ReadMachine = new Game.StageMachine();
            _WriteMachine = new Game.StageMachine();
        }

        bool Utility.IUpdatable.Update()
        {
            _ReadMachine.Update();
            _WriteMachine.Update();
            return true;
        }

        void Framework.ILaunched.Launch()
        {            
            _Tcp = new System.Net.Sockets.TcpClient();
            _Tcp.BeginConnect(_IpAddress, _Port, _OnConnect, null);            
        }

        private void _OnConnect(IAsyncResult ar)
        {            
            _Tcp.EndConnect(ar);                                    
            _Core = new Remoting.AgentCore(this);
            _Core.Initial();

            _ToRead();
            _ToWrite();
        }

        private void _ToRead()
        {
            var stage = new NetworkStreamReadStage(_Tcp.GetStream(), _Tcp.ReceiveBufferSize);
            stage.ReadCompletionEvent += (buffer) =>
            {
                using(var stream = new MemoryStream(buffer))
                {
                    var package = ProtoBuf.Serializer.Deserialize<Package>(stream);                    
                    _Core.OnResponse(package.Code, package.Args);
                }
                _ToRead();
            };
            _ReadMachine.Push(stage);
        }

        private void _ToWrite()
        {
            var stage = new NetworkStreamWriteStage(_Tcp.GetStream(), _WaitWiters);
            stage.WriteCompletionEvent += _ToWrite;
            _WriteMachine.Push(stage);
        }

        void Framework.ILaunched.Shutdown()
        {            
            _Tcp.Close();
            if (_Core != null)
                _Core.Finial();
        }

        void Remoting.IGhostRequest.Request(byte code, Dictionary<byte, object> args)
        {
            _WaitWiters.Enqueue(new Package() { Args = args, Code = code } );
        }

        
    }
}
