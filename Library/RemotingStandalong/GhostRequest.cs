using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Standalong
{
	public class GhostRequest : Regulus.Remoting.IGhostRequest
	{
		class Request
		{
			public byte Code;
			public Dictionary<byte, byte[]> Argments;
		}
		Queue<Request> _Requests;
		
		public GhostRequest ()
		{
			_Requests = new Queue<Request>();
		}
		void Remoting.IGhostRequest.Request(byte code, Dictionary<byte, byte[]> args)
		{
			lock (_Requests)
			{
				_Requests.Enqueue(new Request() {  Code =  code,  Argments =  args });
			}
			
		}

		public void Update()
		{
            Queue<Request> requests = new Queue<Request>();
			lock (_Requests)
			{
				while(_Requests.Count > 0)
				{
                    requests.Enqueue(_Requests.Dequeue());					
				}
			}
            while (requests.Count > 0)
            {
                var request = requests.Dequeue();
                _Apportion(request.Code, request.Argments);
            }            
		}



		private void _Apportion(byte code, Dictionary<byte, byte[]> args)
		{
			if ((int)Regulus.Remoting.ClientToServerPhotonOpCode.Ping == code)
			{
				if(PingEvent != null)
					PingEvent();
			}
			else if( (int) Regulus.Remoting.ClientToServerPhotonOpCode.CallMethod == code)
			{
				var entityId = new Guid(args[0]);

                var methodName = System.Text.Encoding.Default.GetString(args[1]);

				byte[] par = null;				
				Guid returnId = Guid.Empty;
				if (args.TryGetValue(2, out par))
				{
					returnId = new Guid(par);
				}

				var methodParams = (from p in args
									where p.Key >= 3
									orderby p.Key
									select Regulus.PhotonExtension.TypeHelper.Deserialize(p.Value as byte[])).ToArray();			

				if(CallMethodEvent != null)
					CallMethodEvent(entityId, methodName, returnId, methodParams);
			}
			
		}

		
		public event Action<Guid , string , Guid , object[] > CallMethodEvent;
		public event Action PingEvent;
	}
}
