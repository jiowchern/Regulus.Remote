using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samebest.Remoting.Soul
{
    public class ServerPeer : Photon.SocketServer.PeerBase, Regulus.Remoting.IResponseQueue
	{
        readonly ExitGames.Concurrency.Fibers.PoolFiber _Fiber;
		public ServerPeer(Photon.SocketServer.InitRequest initRequest)
			: base(initRequest.Protocol, initRequest.PhotonPeer)
		{
            _Fiber = new ExitGames.Concurrency.Fibers.PoolFiber();
            _Fiber.Start();
		}

		public event Action DisconnectEvent;
		protected override void OnDisconnect(PhotonHostRuntimeInterfaces.DisconnectReason reasonCode, string reasonDetail)
		{
            System.Diagnostics.Debug.WriteLine(reasonCode.ToString());
			if(DisconnectEvent != null)												
				DisconnectEvent.Invoke();


            _Fiber.Stop();
            _Fiber.Dispose();
		}

		public Action<Guid , string ,Guid , object[]>	InvokeMethodEvent;
		
		protected override void OnOperationRequest(Photon.SocketServer.OperationRequest operationRequest, Photon.SocketServer.SendParameters sendParameters)
		{
			if (operationRequest.OperationCode == (byte)ClientToServerPhotonOpCode.Ping)
			{

			}else if (operationRequest.OperationCode == (byte)ClientToServerPhotonOpCode.CallMethod)
			{
				
				var entityId = new Guid(operationRequest.Parameters[0] as byte[]);
				var methodName = operationRequest.Parameters[1] as string;

				object par = null;
				Guid returnId = Guid.Empty;
				if (operationRequest.Parameters.TryGetValue(2, out par))
				{
					returnId = new Guid(par as byte[]);
				}
				
				var methodParams = (from p in operationRequest.Parameters
								   where p.Key >= 3 orderby p.Key
								   select Samebest.PhotonExtension.TypeHelper.Deserialize(p.Value as byte[])).ToArray();

                _Push(entityId, methodName, returnId, methodParams);

				
			}			
		}

        object _Sync = new object();
        class Request
        {
            public Guid EntityId { get; set; }
            public string MethodName { get; set; }
            public Guid ReturnId { get; set; }
            public object[] MethodParams { get; set; }
        }
        Queue<Request> _NewRequests = new Queue<Request>();
        Queue<Request> _UpdateRequests = new Queue<Request>();

        private void _Push(Guid entity_id, string method_name, Guid return_id, object[] method_params)
        {
            lock (_Sync)
            {
                _NewRequests.Enqueue(new Request() { EntityId = entity_id, MethodName = method_name, MethodParams = method_params, ReturnId = return_id });            
            }
        }
                
        class Response
        {
            public byte Id { get; set; }
            public Dictionary<byte, object> Args { get; set; }
            public int Size { get; set; }
        }
        Queue<Response> _NewResponses = new Queue<Response>();
        Queue<Response> _UpdateResponses = new Queue<Response>();

        System.DateTime _UpdateTime;
        internal void Update()
        {
            while (_NewResponses.Count > 0)
            {
                _UpdateResponses.Enqueue(_NewResponses.Dequeue()); 
            }

            lock (_Sync)
            {
                while (_NewRequests.Count > 0)
                {
                    _UpdateRequests.Enqueue(_NewRequests.Dequeue());
                }
            }
            

            if (_UpdateResponses.Count > 0)
            {
                if ((System.DateTime.Now - _UpdateTime).TotalMilliseconds > 20)
                {
                    var cmd = _UpdateResponses.Dequeue();
                    _Fiber.Enqueue(() =>
                    {
                        var op = new Photon.SocketServer.OperationResponse();
                        op.OperationCode = cmd.Id;
                        op.Parameters = cmd.Args;
                        SendOperationResponse(op, new Photon.SocketServer.SendParameters());
                    });                        
                    _UpdateTime = System.DateTime.Now;
                }
            }

            while (_UpdateRequests.Count > 0)
            {
                var cmd = _UpdateRequests.Dequeue();
                if (InvokeMethodEvent != null )
                    InvokeMethodEvent.Invoke(cmd.EntityId, cmd.MethodName, cmd.ReturnId, cmd.MethodParams);
            }

            
        }

        void Regulus.Remoting.IResponseQueue.Push(byte cmd, Dictionary<byte, object> args)
        {
            int size = sizeof(byte);
            foreach (var a in args)
            {
                var b = a.Value as byte[];
                
                size += b.Length;
            }
            var response =  new Response() { Id = cmd, Args = args , Size = size };
            
            _NewResponses.Enqueue(response);                        
        }
    }
}
