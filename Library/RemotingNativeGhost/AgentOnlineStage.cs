using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Ghost.Native
{
    public partial class Agent 
    {
        public static int RequestQueueCount { get { return OnlineStage.RequestQueueCount; } }
        public static int ResponseQueueCount { get { return OnlineStage.ResponseQueueCount; } }
        
        class OnlineStage : Regulus.Utility.IStage, Regulus.Remoting.IGhostRequest
        {
            public event Action DoneEvent;
            
            Regulus.Utility.StageMachine _ReadMachine;
            Regulus.Utility.StageMachine _WriteMachine;




            static object _LockRequest = new object();

            public static int RequestQueueCount { get; private set; }


            static object _LockResponse= new object();

            public static int ResponseQueueCount { get; private set; }
            PackageQueue _Sends;
            PackageQueue _Receives;            
            private System.Net.Sockets.Socket _Socket;
            AgentCore _Core;
            bool _Enable;
            
            public OnlineStage()
            {
                _Sends = new PackageQueue();
                _Receives = new PackageQueue();                
                _Core = new Remoting.AgentCore(this);                
            }
            
            void Utility.IStage.Enter()
            {
                _ReadMachine = new Utility.StageMachine();
                _WriteMachine = new Utility.StageMachine();
               // IOHandler.Instance.Start(this);
                _Enable = true;
                _Core.Initial();
                _ToRead();
                _ToWriteWait();
            }

            void Utility.IStage.Leave()
            {
                _Enable = false;
                _Core.Finial();
                _ReadMachine.Empty();
                _WriteMachine.Empty();
                
               // IOHandler.Instance.Stop(this);
                _ReadMachine.Termination();
                _WriteMachine.Termination();
            }

            void Utility.IStage.Update()
            {
                _WriteMachine.Update();
                _ReadMachine.Update();

                if( !(_Socket.Connected && _Enable) )
                {
                    DoneEvent();
                }
            }
            

            void IGhostRequest.Request(byte code, Dictionary<byte, byte[]> args)
            {                
                _Sends.Enqueue(new Package() { Args = args, Code = code });
                
                lock(_LockRequest)
                {
                    RequestQueueCount++;
                }
                    
            }

            private void _ToRead()
            {
                var stage = new NetworkStreamReadStage(_Socket);
                stage.ReadCompletionEvent += (package) =>
                {
                    
                    _Receives.Enqueue(package);
                
                    lock(_LockResponse )
                    {
                        ResponseQueueCount++;
                    }
                        
                    
                    _ToRead();
                };
                stage.ErrorEvent += () => { _Enable = false; };
                _ReadMachine.Push(stage);
            }

            private void _ToWrite(Package[] packages)
            {
                
                
                var pkgs = packages;
                var requestCount = pkgs.Length;
                lock (_LockRequest)
                {
                    RequestQueueCount -= requestCount;
                }
                        
                var stage = new NetworkStreamWriteStage(_Socket, pkgs);

                stage.WriteCompletionEvent += ()=>
                {
                    _ToWriteWait();
                };
                stage.ErrorEvent += () => 
                {                        
                    _Enable = false; 
                };
                _WriteMachine.Push(stage);
                
                
            }

            private void _ToWriteWait()
            {
                var stage = new WaitQueueStage(_Sends);
                stage.DoneEvent += _ToWrite;
                
                _WriteMachine.Push(stage);
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
                var pkgs = _Receives.DequeueAll();
                lock (_LockResponse)
                    ResponseQueueCount -= pkgs.Length;
                foreach(var pkg in pkgs)
                {
                    _Core.OnResponse(pkg.Code, pkg.Args);
                }

            }
        }
    }
}