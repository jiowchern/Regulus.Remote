// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GhostRequest.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the GhostRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Regulus.Remoting.Standalong
{
	public class GhostRequest : IGhostRequest
	{
		public event Action<Guid, string, Guid, byte[][]> CallMethodEvent;

		public event Action PingEvent;

		public event Action<Guid> ReleaseEvent;

		private readonly Queue<Request> _Requests;

		public GhostRequest()
		{
			this._Requests = new Queue<Request>();
		}

		void IGhostRequest.Request(byte code, Dictionary<byte, byte[]> args)
		{
			lock (this._Requests)
			{
				this._Requests.Enqueue(new Request
				{
					Code = code, 
					Argments = args
				});
			}
		}

		private class Request
		{
			public Dictionary<byte, byte[]> Argments;

			public byte Code;
		}

		public void Update()
		{
			var requests = new Queue<Request>();
			lock (this._Requests)
			{
				while (this._Requests.Count > 0)
				{
					requests.Enqueue(this._Requests.Dequeue());
				}
			}

			while (requests.Count > 0)
			{
				var request = requests.Dequeue();
				this._Apportion(request.Code, request.Argments);
			}
		}

		private void _Apportion(byte code, Dictionary<byte, byte[]> args)
		{
			if ((int)ClientToServerOpCode.Ping == code)
			{
				if (this.PingEvent != null)
				{
					this.PingEvent();
				}
			}
			else if ((int)ClientToServerOpCode.CallMethod == code)
			{
				if (args.Count < 2)
				{
					return;
				}

				var entityId = new Guid(args[0]);

				var methodName = Encoding.Default.GetString(args[1]);

				byte[] par = null;
				var returnId = Guid.Empty;
				if (args.TryGetValue(2, out par))
				{
					returnId = new Guid(par);
				}

				var methodParams = (from p in args
				                    where p.Key >= 3
				                    orderby p.Key
				                    select p.Value).ToArray();

				if (this.CallMethodEvent != null)
				{
					this.CallMethodEvent(entityId, methodName, returnId, methodParams);
				}
			}
			else if ((int)ClientToServerOpCode.Release == code)
			{
				var entityId = new Guid(args[0]);
				this.ReleaseEvent(entityId);
			}
		}
	}
}