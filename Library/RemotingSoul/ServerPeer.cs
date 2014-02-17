using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Soul
{
	public class ServerPeer : Photon.SocketServer.PeerBase, Regulus.Remoting.IResponseQueue, Regulus.Remoting.IRequestQueue
	{
        readonly ExitGames.Concurrency.Fibers.PoolFiber _Fiber;
		public ServerPeer(Photon.SocketServer.InitRequest initRequest)
			: base(initRequest.Protocol, initRequest.PhotonPeer)
		{
            _Fiber = new ExitGames.Concurrency.Fibers.PoolFiber();
            _Fiber.Start();
		}

		public event Action BreakEvent;
		protected override void OnDisconnect(PhotonHostRuntimeInterfaces.DisconnectReason reasonCode, string reasonDetail)
		{
            System.Diagnostics.Debug.WriteLine(reasonCode.ToString());
			if(BreakEvent != null)												
				BreakEvent.Invoke();


            _Fiber.Stop();
            _Fiber.Dispose();
		}

		public event Action<Guid , string ,Guid , byte[][]>	InvokeMethodEvent;
		
		protected override void OnOperationRequest(Photon.SocketServer.OperationRequest operationRequest, Photon.SocketServer.SendParameters sendParameters)
		{
			if (operationRequest.OperationCode == (byte)ClientToServerPhotonOpCode.Ping)
			{
                (this as Regulus.Remoting.IResponseQueue).Push((int)ServerToClientPhotonOpCode.Ping , new Dictionary<byte,byte[]>());
			}else if (operationRequest.OperationCode == (byte)ClientToServerPhotonOpCode.CallMethod)
			{
				
				var entityId = new Guid(operationRequest.Parameters[0] as byte[]);
                var methodName = System.Text.Encoding.Default.GetString(operationRequest.Parameters[1] as byte[]);

				object par = null;
				Guid returnId = Guid.Empty;
				if (operationRequest.Parameters.TryGetValue(2, out par))
				{
					returnId = new Guid(par as byte[]);
				}
				
				var methodParams = (from p in operationRequest.Parameters
								   where p.Key >= 3 orderby p.Key
								   select p.Value as byte[]).ToArray();

                _Push(entityId, methodName, returnId, methodParams);

				
			}			
		}

        object _Sync = new object();
        class Request
        {
            public Guid EntityId { get; set; }
            public string MethodName { get; set; }
            public Guid ReturnId { get; set; }
            public byte[][] MethodParams { get; set; }
        }
        Queue<Request> _NewRequests = new Queue<Request>();
        Queue<Request> _UpdateRequests = new Queue<Request>();

        private void _Push(Guid entity_id, string method_name, Guid return_id, byte[][] method_params)
        {
            lock (_Sync)
            {
                _NewRequests.Enqueue(new Request() { EntityId = entity_id, MethodName = method_name, MethodParams = method_params, ReturnId = return_id });            
            }
        }
                
        class Response
        {
            public byte Id { get; set; }
            public Dictionary<byte, byte[]> Args { get; set; }        
        }
        Queue<Response> _NewResponses = new Queue<Response>();
        Queue<Response> _UpdateResponses = new Queue<Response>();

        
        public void Update()
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
                if (true /*(System.DateTime.Now - _UpdateTime).TotalMilliseconds > 20*/)
                {
                    var cmd = _UpdateResponses.Dequeue();

                    if (cmd == null)
                    {
                        System.Diagnostics.Debug.WriteLine("_UpdateResponses cmd == null");                         
                    }
                    _Fiber.Enqueue(() =>
                    {
                        var command = cmd;
                        var op = new Photon.SocketServer.OperationResponse();
                        op.OperationCode = command.Id;

                        var pars = new Dictionary<byte, object>();
                        foreach (var arg in command.Args)
                        {
                            pars.Add(arg.Key, arg.Value);
                        }
                        op.Parameters = pars;
                        SendOperationResponse(op, new Photon.SocketServer.SendParameters());
                    });                                
                }
            }

            while (_UpdateRequests.Count > 0)
            {
                var cmd = _UpdateRequests.Dequeue();
                if (InvokeMethodEvent != null )
                    InvokeMethodEvent.Invoke(cmd.EntityId, cmd.MethodName, cmd.ReturnId, cmd.MethodParams);
            }

            
        }

        void Regulus.Remoting.IResponseQueue.Push(byte cmd, Dictionary<byte, byte[]> args)
        {
            
            var response =  new Response() { Id = cmd, Args = args };
            
            _NewResponses.Enqueue(response);                        
        }
    }
}
