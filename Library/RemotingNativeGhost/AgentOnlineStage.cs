using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Remoting.Native.Ghost;

namespace Regulus.Remoting.Ghost.Native
{
    public partial class Agent 
    {
        public static int RequestQueueCount { get { return OnlineStage.RequestQueueCount; } }
        public static int ResponseQueueCount { get { return OnlineStage.ResponseQueueCount; } }

        public static int Fps { get { return IOHandler.Instance.Fps; } }
        public static float Power { get { return IOHandler.Instance.Power; } }
        class OnlineStage : Regulus.Utility.IStage, Regulus.Remoting.IGhostRequest
        {
            public event Action DoneEvent;
            
            Regulus.Utility.StageMachine _ReadMachine;
            Regulus.Utility.StageMachine _WriteMachine;


            PackageReader _Reader;
            PackageWriter _Writer;

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
                _Reader = new PackageReader();
                _Writer = new PackageWriter();
                _Sends = new PackageQueue();
                _Receives = new PackageQueue();                
                _Core = new Remoting.AgentCore(this);                
            }
            
            void Utility.IStage.Enter()
            {
                _ReadMachine = new Utility.StageMachine();
                _WriteMachine = new Utility.StageMachine();
                
                _Core.Initial();
                _Enable = true;
                _ReaderStart();
                _WriterStart();
                //_ToWriteWait();
            }

            

            void Utility.IStage.Leave()
            {
                _Core.Finial();
                _WriterStop();
                _ReaderStop();
                _Enable = false;
                
                
                _ReadMachine.Termination();
                _WriteMachine.Termination();
            }

            private void _WriterStop()
            {
                
                _Writer.Stop();
            }

            void Utility.IStage.Update()
            {
                if (!(_Socket.Connected && _Enable))
                {
                    DoneEvent();
                    
                }

                _WriteMachine.Update();
                _ReadMachine.Update();
            }
            

            void IGhostRequest.Request(byte code, Dictionary<byte, byte[]> args)
            {                
                
                
                lock(_LockRequest)
                {
                    _Sends.Enqueue(new Package() { Args = args, Code = code });
                    RequestQueueCount++;
                }
                    
            }
            
            private void _ToRead()
            {
                var stage = new NetworkStreamReadStage(_Socket);
                stage.ReadCompletionEvent += (package) =>
                {

                    _ReceivePackage(package);
                    
                    _ToRead();
                };
                stage.ErrorEvent += () => { _Enable = false; };
                _ReadMachine.Push(stage);
            }

            private void _ReceivePackage(Package package)
            {
                lock (_LockResponse)
                {
                    _Receives.Enqueue(package);
                    ResponseQueueCount++;
                }
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

            
            private void _WriterStart()
            {
                _Writer.Start(_Socket, _Sends);
                _Reader.ErrorEvent += _Disable;
            }
            private void _ReaderStart()
            {
                _Reader.Start(_Socket);
                _Reader.DoneEvent += _ReceivePackage;
                _Reader.ErrorEvent += _Disable;
            }

            private void _Disable()
            {
                _Enable = false;
            }
            
            private void _ReaderStop()
            {
                _Reader.DoneEvent -= _ReceivePackage;
                _Reader.ErrorEvent -= _Disable;
                _Reader.Stop();
            }
        }
    }
}