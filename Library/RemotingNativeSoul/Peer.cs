using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

using Regulus.Serialization;
using Regulus.Framework;
using Regulus.Network;

namespace Regulus.Remoting.Soul.Native
{
	public class Peer : IRequestQueue, IResponseQueue, IBootable
	{
		public delegate void DisconnectCallback();

		private event Action _BreakEvent;

		private event InvokeMethodCallback _InvokeMethodEvent;

		public event DisconnectCallback DisconnectEvent;

		private class Request
		{
			public Guid EntityId { get; set; }

			public int MethodId { get; set; }

			public Guid ReturnId { get; set; }

			public byte[][] MethodParams { get; set; }
		}

		private static readonly object _LockRequest = new object();

		private static readonly object _LockResponse = new object();

		private readonly PackageReader<RequestPackage> _Reader;

		private readonly Regulus.Collection.Queue<RequestPackage> _Requests;

		private readonly Regulus.Collection.Queue<ResponsePackage> _Responses;

		private readonly ISocket _Socket;

	    private readonly IProtocol _Protocol;

	    private readonly SoulProvider _SoulProvider;

		private readonly PackageWriter<ResponsePackage> _Writer;


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
		

		private object _EnableLock;
	    private ISerializer _Serialize;

	    public static bool IsIdle
		{
			get { return Peer.TotalRequest <= 0 && Peer.TotalResponse <= 0; }
		}

		public static int TotalRequest { get; private set; }

		public static int TotalResponse { get; private set; }

		public ISoulBinder Binder
		{
			get { return _SoulProvider; }
		}

		public CoreThreadRequestHandler Handler
		{
			get { return new CoreThreadRequestHandler(this); }
		}

		public Peer(ISocket client , IProtocol protocol)
		{

		    
            _Serialize = protocol.GetSerialize();

		    _EnableLock = new object();

			_Socket = client;
		    _Protocol = protocol;
		    _SoulProvider = new SoulProvider(this, this , protocol);
			_Responses = new Regulus.Collection.Queue<ResponsePackage>();
			_Requests = new Regulus.Collection.Queue<RequestPackage>();

			_Enable = true;

			_Reader = new PackageReader<RequestPackage>(protocol.GetSerialize());
			_Writer = new PackageWriter<ResponsePackage>(protocol.GetSerialize());
		}

		void IBootable.Launch()
		{
			_Reader.DoneEvent += _RequestPush;
			_Reader.ErrorEvent += () => { _Enable = false; };
			_Reader.Start(_Socket);

			_Writer.ErrorEvent += () => { _Enable = false; };
			
			_Writer.Start(_Socket);


            var pkg = new PackageProtocolSubmit();
		    pkg.VerificationCode = _Protocol.VerificationCode;
            _Push(ServerToClientOpCode.ProtocolSubmit, pkg.ToBuffer(_Serialize));

		}

		void IBootable.Shutdown()
		{
			try
			{				
				_Socket.Close();
			}
			catch (System.Net.Sockets.SocketException se)
			{
				Regulus.Utility.Log.Instance.WriteInfo(string.Format("Socket shutdown socket exception.{0}" , se.Message));
			}
			catch (Exception e)
			{
				Regulus.Utility.Log.Instance.WriteInfo(string.Format("Socket shutdown exception.{0}", e.Message));
			}
			
			_Reader.DoneEvent -= _RequestPush;
			_Reader.Stop();
			
			_Writer.Stop();

			lock(Peer._LockResponse)
			{
				var pkgs = _Responses.DequeueAll();
				Peer.TotalResponse -= pkgs.Length;
			}

			lock(Peer._LockRequest)
			{
				var pkgs = _Requests.DequeueAll();
				Peer.TotalRequest -= pkgs.Length;
			}
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

		void IRequestQueue.Update()
		{
			if(_Connected() == false)
			{
				Disconnect();
				DisconnectEvent();
				return;
			}

			_SoulProvider.Update();
			RequestPackage[] pkgs = null;
			lock(Peer._LockRequest)
			{
				pkgs = _Requests.DequeueAll();
				Peer.TotalRequest -= pkgs.Length;
			}

			foreach(var pkg in pkgs)
			{
				var request = _TryGetRequest(pkg);

				if(request != null)
				{
					if(_InvokeMethodEvent != null)
					{
						_InvokeMethodEvent(request.EntityId, request.MethodId, request.ReturnId, request.MethodParams);
					}
				}
			}

		    var responses = _ResponsePop();

            if(responses.Length > 0)
                _Writer.Push(responses);
		}

		void IResponseQueue.Push(ServerToClientOpCode cmd, byte[] data)
		{
		    _Push(cmd, data);
		}

	    private void _Push(ServerToClientOpCode cmd, byte[] data)
	    {
	        lock (Peer._LockResponse)
	        {
	            if (_Enable)
	            {
	                Peer.TotalResponse++;
	                _Responses.Enqueue(
	                    new ResponsePackage
	                    {
	                        Code = cmd,
	                        Data = data
	                    });
	            }
	        }
	    }

	    private void _RequestPush(RequestPackage package)
		{
			lock(Peer._LockRequest)
			{
				_Requests.Enqueue(package);
				Peer.TotalRequest++;
			}
		}

		private Request _TryGetRequest(RequestPackage package)
		{
			if(package.Code == ClientToServerOpCode.Ping)
			{
                _Push(ServerToClientOpCode.Ping, new byte[0]);
				return null;
			}

			if(package.Code == ClientToServerOpCode.CallMethod)
			{
				


			    var data = package.Data.ToPackageData<PackageCallMethod>(_Serialize);                
                return _ToRequest(data.EntityId, data.MethodId, data.ReturnId, data.MethodParams);
			}

			if(package.Code == ClientToServerOpCode.Release)
			{
				//var EntityId = new Guid(package.Args[0]);


                var data = package.Data.ToPackageData<PackageRelease>(_Serialize);
                _SoulProvider.Unbind(data.EntityId);
				return null;
			}

			return null;
		}

		private Request _ToRequest(Guid entity_id, int method_id, Guid return_id, byte[][] method_params)
		{
			return new Request
			{
				EntityId = entity_id, 
				MethodId = method_id, 
				MethodParams = method_params, 
				ReturnId = return_id
			};
		}

		private bool _Connected()
		{
			return _Enable && _Socket.Connected;
		}

		internal void Disconnect()
		{
			if(_BreakEvent != null)
			{
				_BreakEvent();
			}
		}

		private ResponsePackage[] _ResponsePop()
		{
			lock(Peer._LockResponse)
			{
				var pkgs = _Responses.DequeueAll();
				Peer.TotalResponse -= pkgs.Length;
				return pkgs;
			}
		}
	}
}
