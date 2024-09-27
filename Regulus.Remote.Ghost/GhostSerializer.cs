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
    class GhostSerializer : IOpCodeExchangeable
    {
        //private readonly PackageReader<Regulus.Remote.Packages.ResponsePackage> _Reader;
        //private readonly PackageWriter<Regulus.Remote.Packages.RequestPackage> _Writer;

        private readonly Regulus.Network.PackageReader _Reader;
        private readonly Regulus.Network.PackageSender _Sender;
        private readonly IInternalSerializable _Serializable;
        private readonly System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.Packages.ResponsePackage> _Receives;

        private readonly System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.Packages.RequestPackage> _Sends;

        


        
        public GhostSerializer(Regulus.Network.PackageReader reader , PackageSender sender, IInternalSerializable serializable)
        {
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

        event Action<ServerToClientOpCode, Regulus.Memorys.Buffer> IOpCodeExchangeable.ResponseEvent
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

        void IOpCodeExchangeable.Request(ClientToServerOpCode code, Regulus.Memorys.Buffer args)
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
            _ReaderStart();
            
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
            _Process().ContinueWith(t => { }); 
        }

        private async Task _Process()
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
                await _Sender.Send(buf);
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

        private void _ReaderStart()
        {
            _Reader.Read().ContinueWith(_ReadDone);            
        }

        private void _ReadDone(Task<List<Memorys.Buffer>> task)
        {
            if (task.Exception != null)
            {
                Regulus.Utility.Log.Instance.WriteInfo(task.Exception.ToString());                
                return;
            }

            foreach (var buffer in task.Result)
            {
                var pkg = (Packages.ResponsePackage)_Serializable.Deserialize(buffer);
                _Receives.Enqueue(pkg);
            }

            System.Threading.Tasks.Task.Delay(1).ContinueWith(t => _ReaderStart());            
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
