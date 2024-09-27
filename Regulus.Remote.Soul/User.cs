using Regulus.Extensions;
using Regulus.Memorys;
using Regulus.Network;
using Regulus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


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

        private readonly Regulus.Network.PackageReader _Reader;
        private readonly Regulus.Network.PackageSender _Sender;

        private readonly System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.Packages.RequestPackage> _ExternalRequests;
        
        private bool _Enable;

        private readonly IResponseQueue _ResponseQueue;

        public bool Enable => _Enable;
        
        internal readonly IStreamable Stream;
        private readonly IInternalSerializable _InternalSerializer;

        public IBinder Binder
        {
            get { return _SoulProvider; }
        }

        

        public User(IStreamable client, IProtocol protocol , ISerializable serializable, IInternalSerializable internal_serializable , Regulus.Memorys.IPool pool)
        {        
            Stream = client;
            _InternalSerializer = internal_serializable;
            
            _Peer = client;
            _Protocol = protocol;

            _Reader = new Regulus.Network.PackageReader(client , pool);
            _Sender = new Regulus.Network.PackageSender(client , pool);

            _ExternalRequests = new System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.Packages.RequestPackage>();

            _SoulProvider = new SoulProvider(this, this, protocol, serializable, _InternalSerializer);
            _ResponseQueue = this;
            
        }

        void _Launch()
        {
            _Enable = true;

            _StartRead();            
            

            Regulus.Remote.Packages.PackageProtocolSubmit pkg = new Regulus.Remote.Packages.PackageProtocolSubmit();
            pkg.VerificationCode = _Protocol.VerificationCode;

            var buf = _InternalSerializer.Serialize(pkg);
            _ResponseQueue.Push(ServerToClientOpCode.ProtocolSubmit, buf);
        }

        

        private void _StartRead()
        {
            _Reader.Read().ContinueWith(_ReadDone);
        }

        private void _ReadDone(Task<List<Memorys.Buffer>> task)
        {
            if(task.Exception != null)
            {
                Regulus.Utility.Log.Instance.WriteInfo(task.Exception.ToString());
                _Enable = false;
                return;
            }
            foreach(var buffer in task.Result)
            {
                if (buffer.Bytes.Count == 0)
                    continue;
                var pkg = (Packages.RequestPackage)_InternalSerializer.Deserialize(buffer);
                _InternalRequest(pkg);
            }
            
            System.Threading.Tasks.Task.Delay(1).ContinueWith(t => _StartRead());            
        }

        void _Shutdown()
        {
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

        async void IResponseQueue.Push(ServerToClientOpCode cmd, Regulus.Memorys.Buffer buffer)
        {
            if (_Enable)
            {
                await _StartSend(cmd, buffer);

            }
        }

        private async Task _StartSend(ServerToClientOpCode cmd, Memorys.Buffer buffer)
        {
            var pkg = new Regulus.Remote.Packages.ResponsePackage
            {
                Code = cmd,
                Data = buffer.ToArray()
            };
            var buf = _InternalSerializer.Serialize(pkg);
            await _Sender.Send(buf);
        }

        

        private void _ExternalRequest(Regulus.Remote.Packages.RequestPackage package)
        {
            if (package.Code == ClientToServerOpCode.CallMethod)
            {

                Regulus.Remote.Packages.PackageCallMethod data = (Regulus.Remote.Packages.PackageCallMethod)_InternalSerializer.Deserialize(package.Data.AsBuffer())  ;
                var request = _ToRequest(data.EntityId, data.MethodId, data.ReturnId, data.MethodParams);
                _InvokeMethodEvent(request.EntityId, request.MethodId, request.ReturnId, request.MethodParams);
            }
            else if (package.Code == ClientToServerOpCode.AddEvent)
            {
                Regulus.Remote.Packages.PackageAddEvent data = (Regulus.Remote.Packages.PackageAddEvent)_InternalSerializer.Deserialize(package.Data.AsBuffer())  ;
                _SoulProvider.AddEvent(data.Entity, data.Event, data.Handler);
            }
            else if (package.Code == ClientToServerOpCode.RemoveEvent)
            {
                Regulus.Remote.Packages.PackageRemoveEvent data = (Regulus.Remote.Packages.PackageRemoveEvent)_InternalSerializer.Deserialize(package.Data.AsBuffer())  ;
                _SoulProvider.RemoveEvent(data.Entity, data.Event, data.Handler);
            }
            else if (package.Code == ClientToServerOpCode.UpdateProperty)
            {
                Regulus.Remote.Packages.PackageSetPropertyDone data = (Regulus.Remote.Packages.PackageSetPropertyDone)_InternalSerializer.Deserialize(package.Data.AsBuffer());
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
                _ResponseQueue.Push(ServerToClientOpCode.Ping, Regulus.Memorys.Pool.Empty);            
            }            
            else if (package.Code == ClientToServerOpCode.Release)
            {
                Regulus.Remote.Packages.PackageRelease data = (Regulus.Remote.Packages.PackageRelease)_InternalSerializer.Deserialize(package.Data.AsBuffer())  ;
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
 