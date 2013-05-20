using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samebest.Remoting.Soul
{
	
	public class SoulProvider : IDisposable
	{		
		Samebest.Remoting.Soul.ServerPeer	_Peer;

		public SoulProvider(Samebest.Remoting.Soul.ServerPeer peer)
		{
			_Peer = peer;
			_Peer.InvokeMethodEvent += _InvokeMethod;			
		}

        public event Action BreakEvent
        {
            add
            {
                _Peer.DisconnectEvent += value;
            }
            remove
            {
                _Peer.DisconnectEvent -= value;
            }
        }
		class Soul
		{
			public Guid ID { get; set; }
			public object							ObjectInstance	{get;set;}
			public Type								ObjectType		{ get; set; }
			public System.Reflection.MethodInfo[]	MethodInfos		{get;set;}
			public System.Collections.Generic.Dictionary<System.Reflection.EventInfo , Delegate>	EventHandlers { get; set; }			
		}
		System.Collections.Generic.List<Soul>	_Souls = new List<Soul>();

		void _InvokeEvent(Guid entity_id, string event_name, object[] args)
		{
			var op = new Photon.SocketServer.OperationResponse();
			op.OperationCode = (byte)ServerToClientPhotonOpCode.InvokeEvent;
			op.Parameters = new Dictionary<byte, object>();
			op.Parameters.Add(0, entity_id.ToByteArray());
			op.Parameters.Add(1,event_name) ;
			byte i = 2;
			foreach (var arg in args)
			{
				op.Parameters.Add( i , Samebest.PhotonExtension.TypeHelper.Serializer(arg) );
				++i;
			}
			_Peer.SendOperationResponse( op , new Photon.SocketServer.SendParameters());
		}
		
		void _InvokeMethod(Guid entity_id , string method_name ,Guid returnId , object[] args)
		{			
			var soulInfo = (from soul in _Souls where soul.ID == entity_id select new { MethodInfos = soul.MethodInfos , ObjectInstance = soul.ObjectInstance}).FirstOrDefault();
			if (soulInfo != null)
			{
				System.Reflection.MethodInfo methodInfo = (from m in soulInfo.MethodInfos where m.Name == method_name && m.GetParameters().Count() == args.Count() select m).FirstOrDefault();
				if (methodInfo != null)
				{
					var returnValue = methodInfo.Invoke(soulInfo.ObjectInstance, args);
					if (returnId != Guid.Empty)
					{
						_ReturnValue(returnId, returnValue as IValue);							
					}

				}
			}			
		}

		Dictionary<Guid , IValue>	_WaitValues = new Dictionary<Guid,IValue>();
		private void _ReturnValue(Guid returnId, IValue returnValue)
		{
			returnValue.QueryValue(new Action<object>((obj) => 
			{
				var op = new Photon.SocketServer.OperationResponse();
				op.OperationCode = (byte)ServerToClientPhotonOpCode.ReturnValue;
				op.Parameters = new Dictionary<byte, object>();
				op.Parameters.Add(0, returnId.ToByteArray());
				object value = returnValue.GetObject();
				op.Parameters.Add(1, PhotonExtension.TypeHelper.Serializer(value));

				_Peer.SendOperationResponse(op, new Photon.SocketServer.SendParameters());

				_WaitValues.Remove(returnId);
			}));
			_WaitValues.Add(returnId, returnValue);
			
		}

		
		public void Bind<TSoul>(TSoul soul)
		{
			var prevSoul = _Souls.Find((soulInfo) => { return soulInfo.ObjectType == typeof(TSoul); });
			if (prevSoul == null)
			{
				var new_soul = new Soul() { ID = Guid.NewGuid(), ObjectType = typeof(TSoul), ObjectInstance = soul, MethodInfos = typeof(TSoul).GetMethods()};

				Type soulType = typeof(TSoul);
				var eventInfos = soulType.GetEvents();
				new_soul.EventHandlers = new Dictionary<System.Reflection.EventInfo,Delegate>();
				foreach(var eventInfo in eventInfos)
				{					
					var genericArguments =  eventInfo.EventHandlerType.GetGenericArguments();
					Delegate handler = _BuildDelegate(genericArguments, new_soul.ID , eventInfo.Name);					
					new_soul.EventHandlers.Add( eventInfo , handler);
					eventInfo.AddEventHandler(soul , handler);
				}
				_LoadSoul(soulType.FullName, new_soul.ID);
				_Souls.Add(new_soul);
			}
		}
				
		private Delegate _BuildDelegate(Type[] generic_arguments, Guid entity_id, string event_name)
		{
			Type closureType = null;
			Type delegateType = null;
			System.Reflection.MethodInfo run = null;
			Type[] closureTypes = new Type[]	{	typeof(GenericEventClosure)
												,	typeof(GenericEventClosure<>)
												,	typeof(GenericEventClosure<,>)
												,	typeof(GenericEventClosure<,,>)
												,	typeof(GenericEventClosure<,,,>)
												,	typeof(GenericEventClosure<,,,,>)};

			closureType = closureTypes[generic_arguments.Length]; 
			if (generic_arguments.Length != 0)
			{
				closureType = closureType.MakeGenericType(generic_arguments);
				var getDelegateTypeMethod = closureType.GetMethod("GetDelegateType", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
				delegateType = (Type)getDelegateTypeMethod.Invoke(null, null);											
			}
			else 
			{
				delegateType = GenericEventClosure.GetDelegateType();				
			}

			run = closureType.GetMethod("Run");
			object closureInstance = Activator.CreateInstance(closureType, new object[] { entity_id, event_name, new Action<Guid, string, object[]>(_InvokeEvent) });
			return Delegate.CreateDelegate(delegateType, closureInstance, run);
		}

		

		private void _LoadSoul(string type_name , Guid id)
		{
			var op = new Photon.SocketServer.OperationResponse();
			op.OperationCode = (byte)ServerToClientPhotonOpCode.LoadSoul;
			op.Parameters = new Dictionary<byte, object>();
			op.Parameters.Add(0, type_name);
			op.Parameters.Add(1, id.ToByteArray());			
			_Peer.SendOperationResponse(op, new Photon.SocketServer.SendParameters());
		}

		private void _UnloadSoul(string type_name, Guid id)
		{
			var op = new Photon.SocketServer.OperationResponse();
			op.OperationCode = (byte)ServerToClientPhotonOpCode.UnloadSoul;
			op.Parameters = new Dictionary<byte, object>();
			op.Parameters.Add(0, type_name);
			op.Parameters.Add(1, id.ToByteArray());	
			_Peer.SendOperationResponse(op, new Photon.SocketServer.SendParameters());
		}

		public void Unbind<TSoul>(TSoul soul)
		{
			var soulInfo = _Souls.Find((soul_info) => { return soul_info.ObjectInstance  == soul as object; });
			if (soulInfo != null)
			{
				foreach (var eventHandler in soulInfo.EventHandlers)
				{
					eventHandler.Key.RemoveEventHandler(soulInfo.ObjectInstance ,  eventHandler.Value);
				}
				_UnloadSoul(soulInfo.ObjectType.FullName , soulInfo.ID);
				_Souls.Remove(soulInfo);
				
			}
		}

		void IDisposable.Dispose()
		{
			_Peer.InvokeMethodEvent -= _InvokeMethod;
		}
	}
}
