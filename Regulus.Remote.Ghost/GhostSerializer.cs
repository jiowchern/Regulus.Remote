using Regulus.Memorys;
using Regulus.Network;
using Regulus.Serialization;
using Regulus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Regulus.Remote.Ghost
{
    class GhostSerializer : ServerExchangeable
    {
        private readonly Regulus.Network.PackageReader _Reader;
        private readonly Regulus.Network.PackageSender _Sender;
        private readonly IInternalSerializable _Serializable;
        private readonly System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.Packages.ResponsePackage> _Receives;

        
        private readonly System.Collections.Concurrent.ConcurrentBag<System.Exception> _Exceptions;
        public event System.Action<System.Exception> ErrorEvent;
        public GhostSerializer(Regulus.Network.PackageReader reader , PackageSender sender, IInternalSerializable serializable)
        {
            _Exceptions = new System.Collections.Concurrent.ConcurrentBag<Exception>();
            _Reader = reader;
            _Sender = sender;
            this._Serializable = serializable;
        
            _Receives = new System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.Packages.ResponsePackage>();

            _ResponseEvent += _Empty;
        }

        private void _Empty(ServerToClientOpCode arg1, Regulus.Memorys.Buffer arg2)
        {
        }

        event Action<ServerToClientOpCode, Regulus.Memorys.Buffer> _ResponseEvent;

        event Action<ServerToClientOpCode, Regulus.Memorys.Buffer> Exchangeable<ClientToServerOpCode, ServerToClientOpCode>.ResponseEvent
        {
            add
            {
                _ResponseEvent += value;

            }

            remove
            {
                _ResponseEvent -= value;
            }
        }

        void Exchangeable<ClientToServerOpCode, ServerToClientOpCode>.Request(ClientToServerOpCode code, Regulus.Memorys.Buffer args)
        {            
            var buf = _Serializable.Serialize(new Regulus.Remote.Packages.RequestPackage()
            {
                Data = args.ToArray(),
                Code = code
            });
            _Sender.Push(buf);
        }

        public void Start()
        {
            Singleton<Log>.Instance.WriteInfo("Agent online enter.");
            Task.Run( async () => await _ReaderStart());
        }

        public void Stop()
        {
            
            _ReaderStop();
            
            Regulus.Remote.Packages.ResponsePackage val2;
            while (_Receives.TryDequeue(out val2))
            {

            }
            Singleton<Log>.Instance.WriteInfo("Agent online leave.");
        }

        void _Update()
        {
            if(_Exceptions.TryTake( out var e))
            {
                ErrorEvent.Invoke(e);
                return;
            }
            _Process(); 
        }

        private void _Process()
        {
            
            
            while(_Receives.TryDequeue(out var receivePkg))
            {            
                _ResponseEvent(receivePkg.Code, receivePkg.Data.AsBuffer());
            }
        }

       

        private async Task _ReaderStart()
        {

            var packages = await _Reader.Read().ContinueWith(t => { 
                if(t.Exception != null)
                {
                    Singleton<Log>.Instance.WriteInfo($" Agent online error : {t.Exception}");
                    _Exceptions.Add(t.Exception);
                    return new List<Regulus.Memorys.Buffer>();
                }
                return t.Result;
            });
            if(packages.Count == 0)
            {
                _Exceptions.Add(new System.Exception("Agent online error : read 0"));      
                return;
            }
            _ReadDone(packages);
            await System.Threading.Tasks.Task.Delay(0).ContinueWith(t=> _ReaderStart());            
        }

        private void _ReadDone(List<Memorys.Buffer> buffers)
        {
            foreach (var buffer in buffers)
            {
                var pkg = (Packages.ResponsePackage)_Serializable.Deserialize(buffer);
                _Receives.Enqueue(pkg);
            }
        }

        private void _ReaderStop()
        {
         
        }

        public void Update()
        {
            _Update();
        }
    }
}
