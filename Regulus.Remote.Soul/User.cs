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
        
        

        private readonly IResponseQueue _ResponseQueue;

        
        
        internal readonly IStreamable Stream;
        private readonly IInternalSerializable _InternalSerializer;
        public event System.Action ErrorEvent;
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
            
            System.Threading.Tasks.Task.Run(() => _StartRead()).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Regulus.Utility.Log.Instance.WriteInfo(t.Exception.ToString());
                    ErrorEvent();
                }
            });
            /*_StartRead().ContinueWith(t =>
            {
                if(t.Exception != null)
                {
                    Regulus.Utility.Log.Instance.WriteInfo(t.Exception.ToString());
                    ErrorEvent();
                }
            });         */   
            

            Regulus.Remote.Packages.PackageProtocolSubmit pkg = new Regulus.Remote.Packages.PackageProtocolSubmit();
            pkg.VerificationCode = _Protocol.VerificationCode;

            var buf = _InternalSerializer.Serialize(pkg);
            _ResponseQueue.Push(ServerToClientOpCode.ProtocolSubmit, buf);
        }

        

        private async Task _StartRead()
        {
            
            var buffers = await _Reader.Read().ContinueWith(t => {
                System.Collections.Generic.List<Regulus.Memorys.Buffer> result = t.Result;
                t.Exception?.Handle(e =>
                {
                    Regulus.Utility.Log.Instance.WriteInfo($"User _StartRead error {e.ToString()}.");
                    result = new System.Collections.Generic.List<Regulus.Memorys.Buffer>();
                    return true;
                });                
                return result;
            }); 
            _ReadDone(buffers);
            await System.Threading.Tasks.Task.Delay(10).ContinueWith(t => _StartRead());
        }

        private void _ReadDone(List<Regulus.Memorys.Buffer> buffers)
        {
            
            foreach(var buffer in buffers)
            {                
                
                var pkg = (Packages.RequestPackage)_InternalSerializer.Deserialize(buffer);
                _InternalRequest(pkg);
            }
                      
        }

        void _Shutdown()
        {
            //_Updater.Stop();
            Regulus.Remote.Packages.RequestPackage req;
            while (_ExternalRequests.TryDequeue(out req))
            {

            }            
        }

        event InvokeMethodCallback IRequestQueue.InvokeMethodEvent
        {
            add { _InvokeMethodEvent += value; }
            remove { _InvokeMethodEvent -= value; }
        }

        

        void IResponseQueue.Push(ServerToClientOpCode cmd, Regulus.Memorys.Buffer buffer)
        {
            _StartSend(cmd, buffer);
        }

        private void _StartSend(ServerToClientOpCode cmd, Memorys.Buffer buffer)
        {
            var pkg = new Regulus.Remote.Packages.ResponsePackage
            {
                Code = cmd,
                Data = buffer.ToArray()
            };
            var buf = _InternalSerializer.Serialize(pkg);
            _Sender.Send(buf);
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
            else if (package.Code == ClientToServerOpCode.UpdateProperty)
            {
                Regulus.Remote.Packages.PackageSetPropertyDone data = (Regulus.Remote.Packages.PackageSetPropertyDone)_InternalSerializer.Deserialize(package.Data.AsBuffer());
                _SoulProvider.SetPropertyDone(data.EntityId, data.Property);
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
 