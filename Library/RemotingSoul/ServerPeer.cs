using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samebest.Remoting.Soul
{
	public class ServerPeer : Photon.SocketServer.PeerBase
	{
		public ServerPeer(Photon.SocketServer.InitRequest initRequest)
			: base(initRequest.Protocol, initRequest.PhotonPeer)
		{
							
		}

		public event Action DisconnectEvent;
		protected override void OnDisconnect(PhotonHostRuntimeInterfaces.DisconnectReason reasonCode, string reasonDetail)
		{
			if(DisconnectEvent != null)												
				DisconnectEvent.Invoke();
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

				if(InvokeMethodEvent != null)
					InvokeMethodEvent.Invoke(entityId, methodName, returnId, methodParams);
						
			}
			
		}
	}
}
