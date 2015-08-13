
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Standalone

{
	public class GhostRequest : IGhostRequest
	{
		public event Action<Guid, string, Guid, byte[][]> CallMethodEvent;

		public event Action PingEvent;

		public event Action<Guid> ReleaseEvent;

		private class Request
		{
			public Dictionary<byte, byte[]> Argments;

			public byte Code;
		}

		private readonly Queue<Request> _Requests;

		public GhostRequest()
		{
			_Requests = new Queue<Request>();
		}

		void IGhostRequest.Request(byte code, Dictionary<byte, byte[]> args)
		{
			lock(_Requests)
			{
				_Requests.Enqueue(
					new Request
					{
						Code = code, 
						Argments = args
					});
			}
		}

		public void Update()
		{
			var requests = new Queue<Request>();
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
				_Apportion(request.Code, request.Argments);
			}
		}

		private void _Apportion(byte code, Dictionary<byte, byte[]> args)
		{
			if((int)ClientToServerOpCode.Ping == code)
			{
				if(PingEvent != null)
				{
					PingEvent();
				}
			}
			else if((int)ClientToServerOpCode.CallMethod == code)
			{
				if(args.Count < 2)
				{
					return;
				}

				var entityId = new Guid(args[0]);

				var methodName = Encoding.Default.GetString(args[1]);

				byte[] par = null;
				var returnId = Guid.Empty;
				if(args.TryGetValue(2, out par))
				{
					returnId = new Guid(par);
				}

				var methodParams = (from p in args
				                    where p.Key >= 3
				                    orderby p.Key
				                    select p.Value).ToArray();

				if(CallMethodEvent != null)
				{
					CallMethodEvent(entityId, methodName, returnId, methodParams);
				}
			}
			else if((int)ClientToServerOpCode.Release == code)
			{
				var entityId = new Guid(args[0]);
				ReleaseEvent(entityId);
			}
		}
	}
}
