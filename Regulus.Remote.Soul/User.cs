﻿using Regulus.Extensions;
using Regulus.Network;
using Regulus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Remote.Soul
{
    public class User : IRequestQueue, IResponseQueue , Advanceable
    {
        public delegate void DisconnectCallback();

        private event Action _BreakEvent;

        private event InvokeMethodCallback _InvokeMethodEvent;

        private class MethodRequest
        {
            public long EntityId { get; set; }

            public int MethodId { get; set; }

            public long ReturnId { get; set; }

            public byte[][] MethodParams { get; set; }
        }


        private readonly IStreamable _Peer;

        private readonly IProtocol _Protocol;

        private readonly SoulProvider _SoulProvider;

        private readonly PackageReader<Regulus.Remote.Packages.RequestPackage> _Reader;

        private readonly PackageWriter<Regulus.Remote.Packages.ResponsePackage> _Writer;

        private readonly System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.Packages.RequestPackage> _ExternalRequests;
        
        private bool _Enable;

        public bool Enable => _Enable;
        
        internal readonly IStreamable Stream;
        private readonly IInternalSerializable _InternalSerializer;

        public IBinder Binder
        {
            get { return _SoulProvider; }
        }

        

        public User(IStreamable client, IProtocol protocol , ISerializable serializable, IInternalSerializable internal_serializable)
        {
        
            Stream = client;
            _InternalSerializer = internal_serializable;
            
            _Peer = client;
            _Protocol = protocol;
            
            

                        
            _Reader = new PackageReader<Regulus.Remote.Packages.RequestPackage>(_InternalSerializer);
            _Writer = new PackageWriter<Regulus.Remote.Packages.ResponsePackage>(_InternalSerializer);
            
            _ExternalRequests = new System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.Packages.RequestPackage>();

            _SoulProvider = new SoulProvider(this, this, protocol, serializable, _InternalSerializer);

            //_Updater = new ThreadUpdater(_AsyncUpdate);
        }

        void _Launch()
        {
            _Enable = true;
            
            _Reader.DoneEvent += _RequestPush;
            _Reader.ErrorEvent += () => { _Enable = false; };
            _Reader.Start(_Peer);

            _Writer.ErrorEvent += () => { _Enable = false; };
            _Writer.Start(_Peer);

            Regulus.Remote.Packages.PackageProtocolSubmit pkg = new Regulus.Remote.Packages.PackageProtocolSubmit();
            pkg.VerificationCode = _Protocol.VerificationCode;
            
            _Push(ServerToClientOpCode.ProtocolSubmit, _InternalSerializer.Serialize(pkg).ToArray());

            //_Updater.Start();
        }


        void _Shutdown()
        {
            
            
            _Reader.DoneEvent -= _RequestPush;
            _Reader.Stop();

            _Writer.Stop();

            //_Updater.Stop();
            Regulus.Remote.Packages.RequestPackage req;
            while (_ExternalRequests.TryDequeue(out req))
            {

            }

                        

            _SendBreakEvent();
            _Enable = false;
        }

        event InvokeMethodCallback IRequestQueue.InvokeMethodEvent
        {
            add { _InvokeMethodEvent += value; }
            remove { _InvokeMethodEvent -= value; }
        }

        event Action IRequestQueue.BreakEvent
        {
            add { _BreakEvent += value; }
            remove { _BreakEvent -= value; }
        }

       

        void IResponseQueue.Push(ServerToClientOpCode cmd, byte[] data)
        {
            _Push(cmd, data);
        }

        private void _Push(ServerToClientOpCode cmd, byte[] data)
        {
            if (_Enable)
            {
                _Writer.Push(new Regulus.Remote.Packages.ResponsePackage
                {
                    Code = cmd,
                    Data = data
                });                                                
            }
        }

        private void _RequestPush(Regulus.Remote.Packages.RequestPackage package)
        {
            _InternalRequest(package);            
        }

        private void _ExternalRequest(Regulus.Remote.Packages.RequestPackage package)
        {
            if (package.Code == ClientToServerOpCode.CallMethod)
            {

                Regulus.Remote.Packages.PackageCallMethod data = (Regulus.Remote.Packages.PackageCallMethod)_InternalSerializer.Deserialize(package.Data)  ;
                var request = _ToRequest(data.EntityId, data.MethodId, data.ReturnId, data.MethodParams);
                _InvokeMethodEvent(request.EntityId, request.MethodId, request.ReturnId, request.MethodParams);
            }
            else if (package.Code == ClientToServerOpCode.AddEvent)
            {
                Regulus.Remote.Packages.PackageAddEvent data = (Regulus.Remote.Packages.PackageAddEvent)_InternalSerializer.Deserialize(package.Data)  ;
                _SoulProvider.AddEvent(data.Entity, data.Event, data.Handler);
            }
            else if (package.Code == ClientToServerOpCode.RemoveEvent)
            {
                Regulus.Remote.Packages.PackageRemoveEvent data = (Regulus.Remote.Packages.PackageRemoveEvent)_InternalSerializer.Deserialize(package.Data)  ;
                _SoulProvider.RemoveEvent(data.Entity, data.Event, data.Handler);
            }
            else if (package.Code == ClientToServerOpCode.UpdateProperty)
            {
                Regulus.Remote.Packages.PackageSetPropertyDone data = (Regulus.Remote.Packages.PackageSetPropertyDone)_InternalSerializer.Deserialize(package.Data);
                _SoulProvider.SetPropertyDone(data.EntityId, data.Property);
            }
            else
            {
                Regulus.Utility.Log.Instance.WriteInfo($"invalid request code {package.Code}.");
            }
        }
        private void _InternalRequest(Regulus.Remote.Packages.RequestPackage package)
        {

            if (package.Code == ClientToServerOpCode.Ping)
            {
                _Push(ServerToClientOpCode.Ping, new byte[0]);            
            }            
            else if (package.Code == ClientToServerOpCode.Release)
            {
                Regulus.Remote.Packages.PackageRelease data = (Regulus.Remote.Packages.PackageRelease)_InternalSerializer.Deserialize(package.Data)  ;
                _SoulProvider.Unbind(data.EntityId);
                
            }            
            else
            {
                _ExternalRequests.Enqueue(package);
            }
            
        }

        private MethodRequest _ToRequest(long entity_id, int method_id, long return_id, byte[][] method_params)
        {
            return new MethodRequest
            {
                EntityId = entity_id,
                MethodId = method_id,
                MethodParams = method_params,
                ReturnId = return_id
            };
        }

        void _SendBreakEvent()
        {
            if (_BreakEvent != null)
            {
                _BreakEvent();
            }
        }


        public void Launch()
        {
            _Launch();
        }

        public void Shutdown()
        {            
            _Shutdown();
        }

        void Advanceable.Advance()
        {            
            Regulus.Remote.Packages.RequestPackage pkg;
            while (_ExternalRequests.TryDequeue(out pkg))
            {
                _ExternalRequest(pkg);
            }
        }
    }
}
 