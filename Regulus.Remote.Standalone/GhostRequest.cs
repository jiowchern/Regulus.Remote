
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Serialization;
namespace Regulus.Remote.Standalone

{
	public class GhostRequest : IGhostRequest
	{
	    private readonly ISerializer _Serializer;
	    public event InvokeMethodCallback CallMethodEvent;

		public event Action PingEvent;

		public event Action<long> ReleaseEvent;
		public event Action<long, int> SetPropertyDoneEvent;
		public event Action<long,int,long> AddEventEvent;
		public event Action<long, int, long> RemoveEventEvent;




		private readonly Queue<RequestPackage> _Requests;

		public GhostRequest(ISerializer serializer)
		{
		    _Serializer = serializer;
		    _Requests = new Queue<RequestPackage>();
		}

	    void IGhostRequest.Request(ClientToServerOpCode code, byte[] args)
		{
			lock(_Requests)
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
			var requests = new Queue<RequestPackage>();
			lock(_Requests)
			{
				while(_Requests.Count > 0)
				{
					requests.Enqueue(_Requests.Dequeue());
				}
			}

			while(requests.Count > 0)
			{
				var request = requests.Dequeue();
				_Apportion(request.Code, request.Data);
			}
		}

		private void _Apportion(ClientToServerOpCode code, byte[] args)
		{
			if(ClientToServerOpCode.Ping == code)
			{
				if(PingEvent != null)
				{
					PingEvent();
				}
			}
			else if(ClientToServerOpCode.CallMethod == code)
			{
			    var data = args.ToPackageData<PackageCallMethod>(_Serializer);
                if (CallMethodEvent != null)
				{
                    
                    CallMethodEvent(data.EntityId, data.MethodId, data.ReturnId, data.MethodParams);
				}
			}
			else if (ClientToServerOpCode.UpdateProperty == code)
            {
				var data = args.ToPackageData<PackageSetPropertyDone>(_Serializer);
				SetPropertyDoneEvent(data.EntityId,data.Property);
			}
			else if(ClientToServerOpCode.Release == code)
			{
                var data = args.ToPackageData<PackageRelease>(_Serializer);                
				ReleaseEvent(data.EntityId);
			}
			else if (ClientToServerOpCode.AddEvent == code)
            {
				var data = args.ToPackageData<PackageAddEvent>(_Serializer);
				AddEventEvent(data.Entity, data.Event, data.Handler);
			}
			else if (ClientToServerOpCode.RemoveEvent == code)
			{
				var data = args.ToPackageData<PackageRemoveEvent>(_Serializer);
				RemoveEventEvent(data.Entity, data.Event, data.Handler);
			}
		}
	}
}
