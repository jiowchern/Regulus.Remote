using Regulus.Network;
using Regulus.Serialization;
using System;
using System.Collections.Generic;

namespace Regulus.Remote.Soul
{
    public class User : IRequestQueue, IResponseQueue
    {
        public delegate void DisconnectCallback();

        private event Action _BreakEvent;

        private event InvokeMethodCallback _InvokeMethodEvent;

        private class Request
        {
            public long EntityId { get; set; }

            public int MethodId { get; set; }

            public long ReturnId { get; set; }

            public byte[][] MethodParams { get; set; }
        }

        private readonly PackageReader<RequestPackage> _Reader;


        private readonly System.Collections.Concurrent.ConcurrentQueue<RequestPackage> _Requests;

        private readonly System.Collections.Concurrent.ConcurrentQueue<ResponsePackage> _Responses;

        private readonly IStreamable _Peer;

        private readonly IProtocol _Protocol;

        private readonly SoulProvider _SoulProvider;

        private readonly PackageWriter<ResponsePackage> _Writer;

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
        private readonly ISerializer _Serialize;

        public static bool IsIdle
        {
            get { return User.TotalRequest <= 0 && User.TotalResponse <= 0; }
        }

        private static long _TotalRequest;
        public static long TotalRequest { get { return _TotalRequest; } }

        private static long _TotalResponse;
        internal readonly IStreamable Stream;

        public static long TotalResponse { get { return _TotalResponse; } }

        public IBinder Binder
        {
            get { return _SoulProvider; }
        }



        public User(IStreamable client, IProtocol protocol)
        {
            Stream = client;
            _Updater = new ThreadUpdater(_Update);
            _Serialize = protocol.GetSerialize();

            _EnableLock = new object();

            _Peer = client;
            _Protocol = protocol;
            _SoulProvider = new SoulProvider(this, this, protocol);
            _Responses = new System.Collections.Concurrent.ConcurrentQueue<ResponsePackage>();
            _Requests = new System.Collections.Concurrent.ConcurrentQueue<RequestPackage>();
            _Reader = new PackageReader<RequestPackage>(protocol.GetSerialize());
            _Writer = new PackageWriter<ResponsePackage>(protocol.GetSerialize());



        }

        public void Launch()
        {
            _Enable = true;
            _Updater.Start();
            _Reader.DoneEvent += _RequestPush;
            _Reader.ErrorEvent += () => { _Enable = false; };
            _Reader.Start(_Peer);

            _Writer.Start(_Peer);

            

            PackageProtocolSubmit pkg = new PackageProtocolSubmit();
            pkg.VerificationCode = _Protocol.VerificationCode;
            _Push(ServerToClientOpCode.ProtocolSubmit, pkg.ToBuffer(_Serialize));
            
        }

        public void Shutdown()
        {

            _Reader.DoneEvent -= _RequestPush;
            _Reader.Stop();

            _Writer.Stop();

            System.Threading.Interlocked.Add(ref _TotalResponse, -_Responses.Count);
            System.Threading.Interlocked.Add(ref _TotalRequest, -_Requests.Count);


            _Updater.Stop();

            SendBreakEvent();
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

        void _Update()
        {            

            _SoulProvider.Update();

            RequestPackage pkg;
            while (_Requests.TryDequeue(out pkg))
            {
                System.Threading.Interlocked.Decrement(ref _TotalRequest);
                Request request = _TryGetRequest(pkg);

                if (request != null)
                {
                    if (_InvokeMethodEvent != null)
                    {
                        _InvokeMethodEvent(request.EntityId, request.MethodId, request.ReturnId, request.MethodParams);
                    }
                }
            }

            ResponsePackage[] responses = _ResponsePop();

            if (responses.Length > 0)
                _Writer.Push(responses);

            _Writer.Update();
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

        private Request _TryGetRequest(RequestPackage package)
        {


            if (package.Code == ClientToServerOpCode.Ping)
            {
                _Push(ServerToClientOpCode.Ping, new byte[0]);
                return null;
            }
            else if (package.Code == ClientToServerOpCode.CallMethod)
            {
                PackageCallMethod data = package.Data.ToPackageData<PackageCallMethod>(_Serialize);
                return _ToRequest(data.EntityId, data.MethodId, data.ReturnId, data.MethodParams);
            }
            else if (package.Code == ClientToServerOpCode.Release)
            {
                PackageRelease data = package.Data.ToPackageData<PackageRelease>(_Serialize);
                _SoulProvider.Unbind(data.EntityId);
                return null;
            }
            else if (package.Code == ClientToServerOpCode.UpdateProperty)
            {
                PackageSetPropertyDone data = package.Data.ToPackageData<PackageSetPropertyDone>(_Serialize);
                _SoulProvider.SetPropertyDone(data.EntityId, data.Property);
            }
            else if (package.Code == ClientToServerOpCode.AddEvent)
            {
                PackageAddEvent data = package.Data.ToPackageData<PackageAddEvent>(_Serialize);
                _SoulProvider.AddEvent(data.Entity, data.Event, data.Handler);
            }
            else if (package.Code == ClientToServerOpCode.RemoveEvent)
            {
                PackageRemoveEvent data = package.Data.ToPackageData<PackageRemoveEvent>(_Serialize);
                _SoulProvider.RemoveEvent(data.Entity, data.Event, data.Handler);
            }
            else if (ClientToServerOpCode.AddNotifierSupply == package.Code)
            {
                PackageNotifierEventHook data = package.Data.ToPackageData<PackageNotifierEventHook>(_Serialize);
                _SoulProvider.AddNotifierSupply(data.Entity, data.Property, data.Passage);
            }
            else if (ClientToServerOpCode.RemoveNotifierSupply == package.Code)
            {
                PackageNotifierEventHook data = package.Data.ToPackageData<PackageNotifierEventHook>(_Serialize);
                _SoulProvider.RemoveNotifierSupply(data.Entity, data.Property, data.Passage);
            }
            else if (ClientToServerOpCode.AddNotifierUnsupply == package.Code)
            {
                PackageNotifierEventHook data = package.Data.ToPackageData<PackageNotifierEventHook>(_Serialize);
                _SoulProvider.AddNotifierUnsupply(data.Entity, data.Property, data.Passage);
            }
            else if (ClientToServerOpCode.RemoveNotifierUnsupply == package.Code)
            {
                PackageNotifierEventHook data = package.Data.ToPackageData<PackageNotifierEventHook>(_Serialize);
                _SoulProvider.RemoveNotifierUnsupply(data.Entity, data.Property, data.Passage);
            }

            return null;
        }

        private Request _ToRequest(long entity_id, int method_id, long return_id, byte[][] method_params)
        {
            return new Request
            {
                EntityId = entity_id,
                MethodId = method_id,
                MethodParams = method_params,
                ReturnId = return_id
            };
        }




        internal void SendBreakEvent()
        {
            if (_BreakEvent != null)
            {
                _BreakEvent();
            }
        }

        private ResponsePackage[] _ResponsePop()
        {
            List<ResponsePackage> pkgs = new List<ResponsePackage>();
            ResponsePackage pkg;
            while (_Responses.TryDequeue(out pkg))
            {
                pkgs.Add(pkg);
                System.Threading.Interlocked.Decrement(ref _TotalResponse);
            }

            return pkgs.ToArray();
        }
    }
}
