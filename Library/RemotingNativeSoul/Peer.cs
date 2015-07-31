// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Peer.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Peer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

using Regulus.Framework;

#endregion

namespace Regulus.Remoting.Soul.Native
{
	internal class Peer : IRequestQueue, IResponseQueue, IBootable
	{
		private event Action _BreakEvent;

		private event Action<Guid, string, Guid, byte[][]> _InvokeMethodEvent;

		public event DisconnectCallback DisconnectEvent;

		private static readonly object _LockRequest = new object();

		private static readonly object _LockResponse = new object();

		private readonly PackageReader _Reader;

		private readonly PackageQueue _Requests;

		private readonly PackageQueue _Responses;

		private readonly Socket _Socket;

		private readonly SoulProvider _SoulProvider;

		private readonly PackageWriter _Writer;

		private volatile bool _Enable;

		public static bool IsIdle
		{
			get { return Peer.TotalRequest <= 0 && Peer.TotalResponse <= 0; }
		}

		public static int TotalRequest { get; private set; }

		public static int TotalResponse { get; private set; }

		public ISoulBinder Binder
		{
			get { return this._SoulProvider; }
		}

		public CoreThreadRequestHandler Handler
		{
			get { return new CoreThreadRequestHandler(this); }
		}

		public Peer(Socket client)
		{
			this._Socket = client;
			this._SoulProvider = new SoulProvider(this, this);
			this._Responses = new PackageQueue();
			this._Requests = new PackageQueue();

			this._Enable = true;

			this._Reader = new PackageReader();
			this._Writer = new PackageWriter();
		}

		void IBootable.Launch()
		{
			this._Reader.DoneEvent += this._RequestPush;
			this._Reader.ErrorEvent += () => { this._Enable = false; };
			this._Reader.Start(this._Socket);

			this._Writer.ErrorEvent += () => { this._Enable = false; };
			this._Writer.CheckSourceEvent += this._ResponsePop;
			this._Writer.Start(this._Socket);
		}

		void IBootable.Shutdown()
		{
			this._Socket.Shutdown(SocketShutdown.Both);
			this._Socket.Close();
			this._Reader.DoneEvent -= this._RequestPush;
			this._Reader.Stop();
			this._Writer.CheckSourceEvent -= this._ResponsePop;
			this._Writer.Stop();

			lock (Peer._LockResponse)
			{
				var pkgs = this._Responses.DequeueAll();
				Peer.TotalResponse -= pkgs.Length;
			}

			lock (Peer._LockRequest)
			{
				var pkgs = this._Requests.DequeueAll();
				Peer.TotalRequest -= pkgs.Length;
			}
		}

		event Action<Guid, string, Guid, byte[][]> IRequestQueue.InvokeMethodEvent
		{
			add { this._InvokeMethodEvent += value; }
			remove { this._InvokeMethodEvent -= value; }
		}

		event Action IRequestQueue.BreakEvent
		{
			add { this._BreakEvent += value; }
			remove { this._BreakEvent -= value; }
		}

		void IRequestQueue.Update()
		{
			if (this._Connected() == false)
			{
				this.Disconnect();
				this.DisconnectEvent();
				return;
			}

			this._SoulProvider.Update();
			Package[] pkgs = null;
			lock (Peer._LockRequest)
			{
				pkgs = this._Requests.DequeueAll();
				Peer.TotalRequest -= pkgs.Length;
			}

			foreach (var pkg in pkgs)
			{
				var request = this._TryGetRequest(pkg);

				if (request != null)
				{
					if (this._InvokeMethodEvent != null)
					{
						this._InvokeMethodEvent(request.EntityId, request.MethodName, request.ReturnId, request.MethodParams);
					}
				}
			}
		}

		void IResponseQueue.Push(byte cmd, Dictionary<byte, byte[]> args)
		{
			lock (Peer._LockResponse)
			{
				Peer.TotalResponse++;
				this._Responses.Enqueue(new Package
				{
					Code = cmd, 
					Args = args
				});
			}
		}

		private class Request
		{
			public Guid EntityId { get; set; }

			public string MethodName { get; set; }

			public Guid ReturnId { get; set; }

			public byte[][] MethodParams { get; set; }
		}

		public delegate void DisconnectCallback();

		private void _RequestPush(Package package)
		{
			lock (Peer._LockRequest)
			{
				this._Requests.Enqueue(package);
				Peer.TotalRequest++;
			}
		}

		private Request _TryGetRequest(Package package)
		{
			if (package.Code == (byte)ClientToServerOpCode.Ping)
			{
				(this as IResponseQueue).Push((int)ServerToClientOpCode.Ping, new Dictionary<byte, byte[]>());
				return null;
			}

			if (package.Code == (byte)ClientToServerOpCode.CallMethod)
			{
				var entityId = new Guid(package.Args[0]);
				var methodName = Encoding.Default.GetString(package.Args[1]);

				byte[] par = null;
				var returnId = Guid.Empty;
				if (package.Args.TryGetValue(2, out par))
				{
					returnId = new Guid(par);
				}

				var methodParams = (from p in package.Args
				                    where p.Key >= 3
				                    orderby p.Key
				                    select p.Value).ToArray();

				return this._ToRequest(entityId, methodName, returnId, methodParams);
			}

			if (package.Code == (byte)ClientToServerOpCode.Release)
			{
				var entityId = new Guid(package.Args[0]);
				this._SoulProvider.Unbind(entityId);
				return null;
			}

			return null;
		}

		private Request _ToRequest(Guid entity_id, string method_name, Guid return_id, byte[][] method_params)
		{
			return new Request
			{
				EntityId = entity_id, 
				MethodName = method_name, 
				MethodParams = method_params, 
				ReturnId = return_id
			};
		}

		private bool _Connected()
		{
			return this._Enable && this._Socket.Connected;
		}

		internal void Disconnect()
		{
			if (this._BreakEvent != null)
			{
				this._BreakEvent();
			}
		}

		private Package[] _ResponsePop()
		{
			lock (Peer._LockResponse)
			{
				var pkgs = this._Responses.DequeueAll();
				Peer.TotalResponse -= pkgs.Length;
				return pkgs;
			}
		}
	}
}