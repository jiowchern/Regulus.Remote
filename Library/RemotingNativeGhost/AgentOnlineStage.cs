using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Ghost.Native
{
    public partial class Agent 
    {
        class OnlineStage : Regulus.Game.IStage, Regulus.Utility.IUpdatable, Regulus.Remoting.IGhostRequest
        {
            public event Action DoneEvent;
            
            Regulus.Game.StageMachine _ReadMachine;
            Regulus.Game.StageMachine _WriteMachine;
            Queue<Package> _Sends;
            Queue<Package> _Receives;
            private System.Net.Sockets.Socket _Socket;
            AgentCore _Core;
            bool _Enable;
            
            public OnlineStage()
            {                
                _Sends = new Queue<Package>();
                _Receives = new Queue<Package>();                
                _Core = new Remoting.AgentCore(this);                
            }
            
            void Game.IStage.Enter()
            {
                _ReadMachine = new Game.StageMachine();
                _WriteMachine = new Game.StageMachine();
                IOHandler.Instance.Start(this);
                
            }

            void Game.IStage.Leave()
            {
                IOHandler.Instance.Stop(this);
                _ReadMachine.Termination();
                _WriteMachine.Termination();
            }

            void Game.IStage.Update()
            {
                
            }

            bool Utility.IUpdatable.Update()
            {
                while (_Socket.Connected && _Enable)
                {
                    _ReadMachine.Update();
                    _WriteMachine.Update();                    
                    return true;
                }                
                DoneEvent();
                return false;
            }

            void Framework.ILaunched.Launch()
            {
                _Enable = true;
                _Core.Initial();
                _ToRead();
                _ToWrite();
            }

            void Framework.ILaunched.Shutdown()
            {
                _Enable = false;
                _Core.Finial();
                _ReadMachine.Empty();
                _WriteMachine.Empty();
            }

            void IGhostRequest.Request(byte code, Dictionary<byte, byte[]> args)
            {
                lock (_Sends)
                {
                    _Sends.Enqueue(new Package() { Args = args, Code = code });
                }			
            }

            private void _ToRead()
            {
                var stage = new NetworkStreamReadStage(_Socket);
                stage.ReadCompletionEvent += (package) =>
                {
                    lock (_Receives)
                    {
                        _Receives.Enqueue(package);
                    }
                    _ToRead();
                };
                stage.ErrorEvent += () => { _Enable = false; };
                _ReadMachine.Push(stage);
            }

            private void _ToWrite()
            {
                lock (_Sends)
                {
                    if (_Sends.Count > 0)
                    {
                        var package = _Sends.Dequeue();
                        var stage = new NetworkStreamWriteStage(_Socket, package);
                        stage.WriteCompletionEvent += _ToWrite;
                        stage.ErrorEvent += () => { _Enable = false; };
                        _WriteMachine.Push(stage);
                    }
                    else
                    {
                        var stage = new WaitQueueStage(_Sends);
                        stage.DoneEvent += _ToWrite;
                        _WriteMachine.Push(stage);
                    }
                }
            }

            internal void SetSocket(System.Net.Sockets.Socket socket)
            {
                _Socket = socket;
            }

            public long Ping { get { return _Core.Ping; } }

            internal IProviderNotice<T> QueryProvider<T>()
            {
                return _Core.QueryProvider<T>();
            }



            internal void Process()
            {
                lock (_Receives)
                {
                    while (_Receives.Count > 0)
                    {

                        var package = _Receives.Dequeue();
                        _Core.OnResponse(package.Code, package.Args);

                    }
                }
            }
        }
    }
}