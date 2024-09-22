using Regulus.Network;
using Regulus.Serialization;
using Regulus.Utility;
using System;
using System.Collections.Generic;

namespace Regulus.Remote.Ghost
{
    class GhostSerializer : IOpCodeExchangeable
    {
        private readonly PackageReader<Regulus.Remote.Packages.ResponsePackage> _Reader;

        private readonly System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.Packages.ResponsePackage> _Receives;

        private readonly System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.Packages.RequestPackage> _Sends;

        private readonly PackageWriter<Regulus.Remote.Packages.RequestPackage> _Writer;


        
        public GhostSerializer(PackageReader<Regulus.Remote.Packages.ResponsePackage> reader , PackageWriter<Regulus.Remote.Packages.RequestPackage> writer)
        {
            _Reader = reader;
            _Writer = writer;
            _Sends = new System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.Packages.RequestPackage>();
            _Receives = new System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.Packages.ResponsePackage>();

            _ResponseEvent += _Empty;
        }

        private void _Empty(ServerToClientOpCode arg1, byte[] arg2)
        {
        }

        event Action<ServerToClientOpCode, byte[]> _ResponseEvent;

        event Action<ServerToClientOpCode, byte[]> IOpCodeExchangeable.ResponseEvent
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

        void IOpCodeExchangeable.Request(ClientToServerOpCode code, byte[] args)
        {
            _Sends.Enqueue(
                    new Regulus.Remote.Packages.RequestPackage()
                    {
                        Data = args,
                        Code = code
                    });            
        }

        public void Start(IStreamable peer)
        {
            Singleton<Log>.Instance.WriteInfo("Agent online enter.");
            _ReaderStart(peer);
            _WriterStart(peer);
        }

        public void Stop()
        {
            _WriterStop();
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
            _Process();
        }

        private void _ReceivePackage(Regulus.Remote.Packages.ResponsePackage package)
        {
            _Receives.Enqueue(package);
            
        }

        private void _Process()
        {
            
            Regulus.Remote.Packages.ResponsePackage receivePkg;
            while(_Receives.TryDequeue(out receivePkg))
            {            
                _ResponseEvent(receivePkg.Code, receivePkg.Data);
            }

            Regulus.Remote.Packages.RequestPackage[] sends = _SendsPop();
            if (sends.Length > 0)
                _Writer.Push(sends);
            
        }

        private void _WriterStart(IStreamable peer)
        {
            _Writer.Start(peer);
        }

        private void _WriterStop()
        {
            

            _Writer.Stop();
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

        private void _ReaderStart(IStreamable peer)
        {
            _Reader.DoneEvent += _ReceivePackage;

            
            _Reader.Start(peer);
        }

     

        private void _ReaderStop()
        {
            _Reader.DoneEvent -= _ReceivePackage;
            
            _Reader.Stop();
        }

        public void Update()
        {
            _Update();
        }
    }
}
