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

        private readonly System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.Packages.RequestPackage> _Sends;
        private readonly System.Collections.Concurrent.ConcurrentBag<System.Exception> _Exceptions;
        public event System.Action<System.Exception> ErrorEvent;
        public GhostSerializer(Regulus.Network.PackageReader reader , PackageSender sender, IInternalSerializable serializable)
        {
            _Exceptions = new System.Collections.Concurrent.ConcurrentBag<Exception>();
            _Reader = reader;
            _Sender = sender;
            this._Serializable = serializable;
            _Sends = new System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.Packages.RequestPackage>();
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
            _Sends.Enqueue(
                    new Regulus.Remote.Packages.RequestPackage()
                    {
                        Data = args.ToArray(),
                        Code = code
                    });            
        }

        public void Start()
        {
            Singleton<Log>.Instance.WriteInfo("Agent online enter.");
            _ReaderStart().ContinueWith(t => { 
                if(t.Exception != null)
                {
                    Singleton<Log>.Instance.WriteInfo($" Agent online error : {t.Exception}");
                    _Exceptions.Add(t.Exception);
                }
            });
            
        }

        public void Stop()
        {
            
            _ReaderStop();
            Regulus.Remote.Packages.RequestPackage val;
            Regulus.Remote.Packages.ResponsePackage val2;
            while (_Sends.TryDequeue(out val) ||　_Receives.TryDequeue(out val2))
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
            
            Regulus.Remote.Packages.ResponsePackage receivePkg;
            while(_Receives.TryDequeue(out receivePkg))
            {            
                _ResponseEvent(receivePkg.Code, receivePkg.Data.AsBuffer());
            }

            Regulus.Remote.Packages.RequestPackage[] sends = _SendsPop();
            foreach (var send in sends)
            {
                var buf = _Serializable.Serialize(send);
                _Sender.Send(buf);
            }
                
            
        }

        private Regulus.Remote.Packages.RequestPackage[] _SendsPop()
        {           

            List<Regulus.Remote.Packages.RequestPackage> pkgs = new List<Regulus.Remote.Packages.RequestPackage>();
            Regulus.Remote.Packages.RequestPackage pkg;
            while(_Sends.TryDequeue(out pkg))
            {
                pkgs.Add(pkg);
                
            }
            return pkgs.ToArray();
        }

        private async Task _ReaderStart()
        {
            
            var buffers = await _Reader.Read();
            _ReadDone(buffers);
            await System.Threading.Tasks.Task.Delay(10).ContinueWith(t=> _ReaderStart().ContinueWith(t1 => {
                if (t1.Exception != null)
                {
                    Singleton<Log>.Instance.WriteInfo($" Agent online error : {t1.Exception}");
                    _Exceptions.Add(t.Exception);
                }
            }));            
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
            _Reader.Stop();
        }

        public void Update()
        {
            _Update();
        }
    }
}
