using System;
using System.Net;
using Regulus.Network.RUDP;
using Regulus.Utility;
using Console = Regulus.Utility.Console;

namespace Regulus.Network.Tests.AgentApp
{
    internal class InitialStage : IStage
    {
        private readonly Command _Command;
        private readonly Console.IViewer _Viewer;
        private ISocketClient _Agent;
        private ISocketConnectable _Peer;
        public event Action<ISocket> CreatedEvent;
        public InitialStage(ISocketClient agent , Command command,Console.IViewer viewer)
        {
            _Agent = agent;
            _Command = command;
            _Viewer = viewer;
            
            _Peer = _Agent.Spawn();

        }

        void IStage.Enter()
        {
            _Command.RegisterLambda<InitialStage, string,int>(this, (obj, ip,port) => obj.Connect(ip,port));
            _Command.RegisterLambda<InitialStage>(this, (obj) => obj.Run());
            _Command.RegisterLambda<InitialStage>(this, (obj) => obj.Run2());
        }

        private void Run()
        {
            Connect("127.0.0.1",12345);
        }

        private void Run2()
        {
            Connect("122.116.116.154", 47536);
        }

        private void Connect(string ip,int port)
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _Peer.Connect(endPoint,_ConnectResult );


        }

        private void _ConnectResult(bool result)
        {
            if(result)
                CreatedEvent(_Peer);
            else
            {
                _Viewer.WriteLine("connect timeout.");
            }
        }

        void IStage.Leave()
        {
            _Command.Unregister("Connect");
            _Command.Unregister("Run");
            _Command.Unregister("Run2");
        }

        void IStage.Update()
        {            
        }
    }
}