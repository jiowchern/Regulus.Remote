
using Regulus.Serialization;
using System;
using System.Collections.Generic;
namespace Regulus.Remote.Standalone

{
    public class GhostRequest : IGhostRequest
    {
        private readonly ISerializer _Serializer;
        public event InvokeMethodCallback CallMethodEvent;

        public event Action PingEvent;

        public event Action<long> ReleaseEvent;
        public event Action<long, int> SetPropertyDoneEvent;
        public event Action<long, int, long> AddEventEvent;
        public event Action<long, int, long> RemoveEventEvent;

        public event Action<long, int, long> AddNotifierSupplyEvent;
        public event Action<long, int, long> RemoveNotifierSupplyEvent;
        public event Action<long, int, long> AddNotifierUnsupplyEvent;
        public event Action<long, int, long> RemoveNotifierUnsupplyEvent;






        private readonly Queue<RequestPackage> _Requests;

        public GhostRequest(ISerializer serializer)
        {
            _Serializer = serializer;
            _Requests = new Queue<RequestPackage>();
        }

        event Action<ServerToClientOpCode, byte[]> IGhostRequest.ResponseEvent
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        void IGhostRequest.Request(ClientToServerOpCode code, byte[] args)
        {
            lock (_Requests)
            {
                _Requests.Enqueue(
                    new RequestPackage
                    {
                        Code = code,
                        Data = args
                    });
            }
        }

        public void Update()
        {
            Queue<RequestPackage> requests = new Queue<RequestPackage>();
            lock (_Requests)
            {
                while (_Requests.Count > 0)
                {
                    requests.Enqueue(_Requests.Dequeue());
                }
            }

            while (requests.Count > 0)
            {
                RequestPackage request = requests.Dequeue();
                _Apportion(request.Code, request.Data);
            }
        }

        private void _Apportion(ClientToServerOpCode code, byte[] args)
        {
            if (ClientToServerOpCode.Ping == code)
            {
                if (PingEvent != null)
                {
                    PingEvent();
                }
            }
            else if (ClientToServerOpCode.CallMethod == code)
            {
                PackageCallMethod data = args.ToPackageData<PackageCallMethod>(_Serializer);
                if (CallMethodEvent != null)
                {

                    CallMethodEvent(data.EntityId, data.MethodId, data.ReturnId, data.MethodParams);
                }
            }
            else if (ClientToServerOpCode.UpdateProperty == code)
            {
                PackageSetPropertyDone data = args.ToPackageData<PackageSetPropertyDone>(_Serializer);
                SetPropertyDoneEvent(data.EntityId, data.Property);
            }
            else if (ClientToServerOpCode.Release == code)
            {
                PackageRelease data = args.ToPackageData<PackageRelease>(_Serializer);
                ReleaseEvent(data.EntityId);
            }
            else if (ClientToServerOpCode.AddEvent == code)
            {
                PackageAddEvent data = args.ToPackageData<PackageAddEvent>(_Serializer);
                AddEventEvent(data.Entity, data.Event, data.Handler);
            }
            else if (ClientToServerOpCode.RemoveEvent == code)
            {
                PackageRemoveEvent data = args.ToPackageData<PackageRemoveEvent>(_Serializer);
                RemoveEventEvent(data.Entity, data.Event, data.Handler);
            }
            else if (ClientToServerOpCode.AddNotifierSupply == code)
            {
                PackageNotifierEvent data = args.ToPackageData<PackageNotifierEvent>(_Serializer);
                AddNotifierSupplyEvent(data.Entity, data.Property, data.Passage);
            }
            else if (ClientToServerOpCode.RemoveNotifierSupply == code)
            {
                PackageNotifierEvent data = args.ToPackageData<PackageNotifierEvent>(_Serializer);
                RemoveNotifierSupplyEvent(data.Entity, data.Property, data.Passage);
            }
            else if (ClientToServerOpCode.AddNotifierUnsupply == code)
            {
                PackageNotifierEvent data = args.ToPackageData<PackageNotifierEvent>(_Serializer);
                AddNotifierUnsupplyEvent(data.Entity, data.Property, data.Passage);
            }
            else if (ClientToServerOpCode.RemoveNotifierUnsupply == code)
            {
                PackageNotifierEvent data = args.ToPackageData<PackageNotifierEvent>(_Serializer);
                RemoveNotifierUnsupplyEvent(data.Entity, data.Property, data.Passage);
            }



        }
    }
}
