using Regulus.Network;
using Regulus.Utility;
using System;
using System.Collections.Generic;

namespace Regulus.Remote.Soul
{
    public class User : IRequestQueue, IResponseQueue , Regulus.Utility.IUpdatable
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

        


        private readonly System.Collections.Concurrent.ConcurrentQueue<RequestPackage> _Requests;

        private readonly System.Collections.Concurrent.ConcurrentQueue<ResponsePackage> _Responses;

        private readonly IStreamable _Peer;

        private readonly IProtocol _Protocol;

        private readonly SoulProvider _SoulProvider;

        private readonly PackageReader<RequestPackage> _Reader;

        private readonly PackageWriter<ResponsePackage> _Writer;

        private readonly System.Collections.Concurrent.ConcurrentQueue<RequestPackage> _ExternalRequests;

        readonly ThreadUpdater _Updater;

        private volatile bool _EnableValue;
        private bool _Enable
        {
            get
            {
                lock (_EnableLock)
                {
                    return _EnableValue;
                }
            }

            set
            {
                lock (_EnableLock)
                {
                    _EnableValue = value;
                }
            }

        }



        private readonly object _EnableLock;
        private readonly ISerializable _Serialize;

        public static bool IsIdle
        {
            get { return User.TotalRequest <= 0 && User.TotalResponse <= 0; }
        }

        private static long _TotalRequest;
        public static long TotalRequest { get { return _TotalRequest; } }

        private static long _TotalResponse;
        internal readonly IStreamable Stream;
        private readonly IInternalSerializable _InternalSerializer;

        public static long TotalResponse { get { return _TotalResponse; } }

        public IBinder Binder
        {
            get { return _SoulProvider; }
        }

        public readonly object State;

        public User(IStreamable client, IProtocol protocol , ISerializable serializable, IInternalSerializable internal_serializable, object state)
        {
            State = state;
            Stream = client;
            _InternalSerializer = internal_serializable;
            _Serialize = serializable;

            _EnableLock = new object();

            _Peer = client;
            _Protocol = protocol;
            
            

            _Responses = new System.Collections.Concurrent.ConcurrentQueue<ResponsePackage>();
            _Requests = new System.Collections.Concurrent.ConcurrentQueue<RequestPackage>();
            _Reader = new PackageReader<RequestPackage>(_InternalSerializer);
            _Writer = new PackageWriter<ResponsePackage>(_InternalSerializer);
            
            _ExternalRequests = new System.Collections.Concurrent.ConcurrentQueue<RequestPackage>();

            _SoulProvider = new SoulProvider(this, this, protocol, serializable, _InternalSerializer);

            _Updater = new ThreadUpdater(_AsyncUpdate);
        }

        void _Launch()
        {
            _Enable = true;
            
            _Reader.DoneEvent += _RequestPush;
            _Reader.ErrorEvent += () => { _Enable = false; };
            _Reader.Start(_Peer);

            _Writer.ErrorEvent += () => { _Enable = false; };
            _Writer.Start(_Peer);

            PackageProtocolSubmit pkg = new PackageProtocolSubmit();
            pkg.VerificationCode = _Protocol.VerificationCode;
            _Push(ServerToClientOpCode.ProtocolSubmit, pkg.ToBuffer(_InternalSerializer));

            _Updater.Start();
        }


        void _Shutdown()
        {
            
            
            _Reader.DoneEvent -= _RequestPush;
            _Reader.Stop();

            _Writer.Stop();

            _Updater.Stop();
            RequestPackage req;
            while (_ExternalRequests.TryDequeue(out req))
            {

            }

            System.Threading.Interlocked.Add(ref _TotalResponse, -_Responses.Count);
            System.Threading.Interlocked.Add(ref _TotalRequest, -_Requests.Count);

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

        void _AsyncUpdate()
        {

            _SoulProvider.Update();

            _Writer.Push(_ResponsePop());
            
            _Reader.Update();

            _ExecuteRequests();
        }
        
        private void _ExecuteRequests()
        {
            RequestPackage pkg;
            while (_Requests.TryDequeue(out pkg))
            {
                System.Threading.Interlocked.Decrement(ref _TotalRequest);
                _InternalRequest(pkg);
            }
        }

        void IResponseQueue.Push(ServerToClientOpCode cmd, byte[] data)
        {
            _Push(cmd, data);
        }

        private void _Push(ServerToClientOpCode cmd, byte[] data)
        {
            if (_Enable)
            {
                System.Threading.Interlocked.Increment(ref _TotalResponse);

                _Responses.Enqueue(
                    new ResponsePackage
                    {
                        Code = cmd,
                        Data = data
                    });
            }
        }

        private void _RequestPush(RequestPackage package)
        {
            System.Threading.Interlocked.Increment(ref _TotalRequest);

            _Requests.Enqueue(package);
        }

        private void _ExternalRequest(RequestPackage package)
        {
            if (package.Code == ClientToServerOpCode.CallMethod)
            {
                PackageCallMethod data = package.Data.ToPackageData<PackageCallMethod>(_InternalSerializer);
                var request = _ToRequest(data.EntityId, data.MethodId, data.ReturnId, data.MethodParams);
                _InvokeMethodEvent(request.EntityId, request.MethodId, request.ReturnId, request.MethodParams);
            }
            else if (package.Code == ClientToServerOpCode.AddEvent)
            {
                PackageAddEvent data = package.Data.ToPackageData<PackageAddEvent>(_InternalSerializer);
                _SoulProvider.AddEvent(data.Entity, data.Event, data.Handler);
            }
            else if (package.Code == ClientToServerOpCode.RemoveEvent)
            {
                PackageRemoveEvent data = package.Data.ToPackageData<PackageRemoveEvent>(_InternalSerializer);
                _SoulProvider.RemoveEvent(data.Entity, data.Event, data.Handler);
            }
            else
            {
                Regulus.Utility.Log.Instance.WriteInfo($"invalid request code {package.Code}.");
            }
        }
        private void _InternalRequest(RequestPackage package)
        {

            if (package.Code == ClientToServerOpCode.Ping)
            {
                _Push(ServerToClientOpCode.Ping, new byte[0]);            
            }            
            else if (package.Code == ClientToServerOpCode.Release)
            {
                PackageRelease data = package.Data.ToPackageData<PackageRelease>(_InternalSerializer);
                _SoulProvider.Unbind(data.EntityId);
                
            }
            else if (package.Code == ClientToServerOpCode.UpdateProperty)
            {
                PackageSetPropertyDone data = package.Data.ToPackageData<PackageSetPropertyDone>(_InternalSerializer);
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

        void _SendBreakEvent()
        {
            if (_BreakEvent != null)
            {
                _BreakEvent();
            }
        }

        private IEnumerable<ResponsePackage>  _ResponsePop()
        {            
            ResponsePackage pkg;
            while (_Responses.TryDequeue(out pkg))
            {
                System.Threading.Interlocked.Decrement(ref _TotalResponse);
                yield return pkg;
            }
        }

        bool IUpdatable.Update()
        {
            RequestPackage pkg;
            while (_ExternalRequests.TryDequeue(out pkg))
            {
                _ExternalRequest(pkg);
            }
            
            return _Enable;
        }

        void IBootable.Launch()
        {
            _Launch();
        }

        void IBootable.Shutdown()
        {
            
            _Shutdown();
        }
    }
}
