using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Remote
{
    public partial class SoulProvider : IDisposable, IBinder
	{
		private readonly IdLandlord _IdLandlord;
		private readonly Queue<byte[]> _EventFilter = new Queue<byte[]>();

		private readonly IRequestQueue _Peer;

		private readonly IResponseQueue _Queue;

	    private readonly IProtocol _Protocol;

	    private readonly EventProvider _EventProvider;

		private readonly Poller<Soul> _Souls = new Poller<Soul>();

		private readonly Dictionary<long, IValue> _WaitValues = new Dictionary<long, IValue>();

		private readonly ISerializer _Serializer;

		public SoulProvider(IRequestQueue peer, IResponseQueue queue , IProtocol protocol)
		{
			_IdLandlord = new IdLandlord();
			_Queue = queue;
		    _Protocol = protocol;

		    _EventProvider = protocol.GetEventProvider();

			_Serializer =  protocol.GetSerialize();
			_Peer = peer;
			_Peer.InvokeMethodEvent += _InvokeMethod;
		}

		public void Dispose()
		{
			_Peer.InvokeMethodEvent -= _InvokeMethod;
		}

		void IBinder.Return<TSoul>(TSoul soul)
		{
			if(soul == null)
			{
				throw new ArgumentNullException("soul");
			}

			_Bind(soul, true,0);
		}

		void IBinder.Bind<TSoul>(TSoul soul)
		{
			if(soul == null)
			{
				throw new ArgumentNullException("soul");
			}

			_Bind(soul, false, 0);
		}

		void IBinder.Unbind<TSoul>(TSoul soul)
		{
			if(soul == null)
			{
				throw new ArgumentNullException("soul");
			}

			_Unbind(soul, typeof(TSoul));
		}

		event Action IBinder.BreakEvent
		{
			add
			{
				lock(_Peer)
				{
					_Peer.BreakEvent += value;
				}
			}

			remove
			{
				lock(_Peer)
				{
					_Peer.BreakEvent -= value;
				}
			}
		}



		private void _InvokeEvent(long entity_id, int event_id,long handler_id, object[] args)
		{

		    
            
            var package = new PackageInvokeEvent();
			package.EntityId = entity_id;
			package.Event = event_id;
			package.HandlerId = handler_id;
			package.EventParams = (from a in args select _Serializer.Serialize(a)).ToArray();
			_InvokeEvent(package.ToBuffer(_Serializer));
		}

		private void _InvokeEvent(byte[] argmants)
		{
			lock(_EventFilter)
			{
				_EventFilter.Enqueue(argmants);
			}
		}

		private void _ReturnValue(long returnId, IValue returnValue)
		{
			IValue outValue = null;
			if(_WaitValues.TryGetValue(returnId, out outValue))
			{
				return;
			}

			_WaitValues.Add(returnId, returnValue);
			returnValue.QueryValue(
				obj =>
				{
					if(returnValue.IsInterface() == false)
					{
						_ReturnDataValue(returnId, returnValue);
					}
					else
					{
						_ReturnSoulValue(returnId, returnValue);
					}

					_WaitValues.Remove(returnId);					
				});
		}

		private void _ReturnSoulValue(long return_id, IValue returnValue)
		{
			var soul = returnValue.GetObject();
			var type = returnValue.GetObjectType();
			var prevSoul = (from soulInfo in _Souls.UpdateSet()
							where object.ReferenceEquals(soulInfo.ObjectInstance, soul) && soulInfo.ObjectType == type
							select soulInfo).SingleOrDefault();

			if(prevSoul == null)
			{
				var new_soul = _NewSoul(soul, type);

				_LoadSoul(new_soul.InterfaceId, new_soul.Id, true);

				_LoadProperty(new_soul);

				_LoadSoulCompile(new_soul.InterfaceId, new_soul.Id, return_id);
			}
		}

		private void _LoadProperty(Soul new_soul)
		{
			
			var propertys = new_soul.ObjectType.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
			var map = _Protocol.GetMemberMap();
			for (var i = 0; i < propertys.Length; ++i)
			{
				var property = propertys[i];
				var id = map.GetProperty(property);

				if (property.PropertyType.GetInterfaces().Any(t => t == typeof(IDirtyable)))
				{
					var propertyValue = property.GetValue(new_soul.ObjectInstance);
					
					var accessable = propertyValue as IAccessable;					
					_LoadProperty(new_soul.Id , id , accessable.Get());
				}
				
			}
		}

        

        private void _ReturnDataValue(long returnId, IValue returnValue)
		{
			var value = returnValue.GetObject();
			var package = new PackageReturnValue();
			package.ReturnTarget = returnId;
			package.ReturnValue = _Serializer.Serialize(value);
			_Queue.Push(ServerToClientOpCode.ReturnValue, package.ToBuffer(_Serializer));
		}

		private void _LoadSoulCompile(int type_id, long id, long return_id)
		{
			var package = new PackageLoadSoulCompile();
			package.EntityId = id;
			package.ReturnId = return_id;
			package.TypeId = type_id;

			_Queue.Push(ServerToClientOpCode.LoadSoulCompile, package.ToBuffer(_Serializer));
		}
		private void _LoadProperty(long id, int property,object val)
		{
			var package = new PackageSetProperty();
			package.EntityId = id;
			package.Property = property;
			package.Value = _Serializer.Serialize(val);
			_Queue.Push(ServerToClientOpCode.SetProperty, package.ToBuffer(_Serializer));
		}
		private void _LoadSoul(int type_id, long id, bool return_type)
		{
			var package = new PackageLoadSoul();
			package.TypeId = type_id;
			package.EntityId = id;
			package.ReturnType = return_type;
			_Queue.Push(ServerToClientOpCode.LoadSoul, package.ToBuffer(_Serializer));

			
		}

		private void _UnloadSoul(int type_id, long id)
		{			

			var package = new PackageUnloadSoul();
			package.TypeId = type_id;
			package.EntityId = id;
			_Queue.Push(ServerToClientOpCode.UnloadSoul, package.ToBuffer(_Serializer));
		}

        public void SetPropertyDone(long entityId, int property)
        {
			var souls = from soul in _Souls.UpdateSet() where soul.Id == entityId select soul;
			var s = souls.FirstOrDefault();
			if(s!= null)
            {
				s.PropertyUpdateReset(property);				
			}
				
		}

        private void _InvokeMethod(long entity_id, int method_id, long returnId, byte[][] args)
		{
			var soulInfo = (from soul in _Souls.UpdateSet()
							where soul.Id == entity_id
							select new
							{                                
								soul.MethodInfos, 
								soul.ObjectInstance
							}).FirstOrDefault();
			if(soulInfo != null)
			{
			    var info = _Protocol.GetMemberMap().GetMethod(method_id);
				var methodInfo =
					(from m in soulInfo.MethodInfos where m == info && m.GetParameters().Count() == args.Count() select m)
						.FirstOrDefault();
				if(methodInfo != null)
				{					

					try
					{
						
						var argObjects = args.Select(arg => _Serializer.Deserialize(arg));

						var returnValue = methodInfo.Invoke(soulInfo.ObjectInstance, argObjects.ToArray());
						if(returnValue != null)
						{
							_ReturnValue(returnId, returnValue as IValue);
						}
					}
					catch(DeserializeException deserialize_exception)
					{
						var message  =  deserialize_exception.Base.ToString();                        
						_ErrorDeserialize(method_id.ToString(), returnId , message);
					}
					catch(Exception e)
					{
						Log.Instance.WriteDebug(e.ToString());
						_ErrorDeserialize(method_id.ToString(), returnId, e.Message);
					}
					
				}
			}
		}
		public void RemoveEvent(long entity_id, int event_id, long handler_id)
		{
			var soul = (from s in _Souls.UpdateSet() where s.Id == entity_id select s).FirstOrDefault();
			if (soul == null)
				return;

			var eventInfo = _Protocol.GetMemberMap().GetEvent(event_id);
			if (eventInfo == null)
				return;

			if (eventInfo.DeclaringType != soul.ObjectType)
				return;

			soul.RemoveEvent(eventInfo, handler_id);
		}

		public void AddEvent(long entity_id, int event_id, long handler_id)
        {
			var soul = (from s in _Souls.UpdateSet() where s.Id == entity_id select s).FirstOrDefault();
			if (soul == null)
				return;

			var eventInfo = _Protocol.GetMemberMap().GetEvent(event_id);
			if (eventInfo == null)
				return;
			if (eventInfo.DeclaringType != soul.ObjectType)
				return;

			var del = _BuildDelegate(eventInfo, soul.Id, handler_id, _InvokeEvent);

			var handler = new SoulProvider.Soul.EventHandler(soul.ObjectInstance, del, eventInfo , handler_id);
			soul.AddEvent(handler);

		}

		public void AddNotifier(long entity_id, int property_id, long handler_id)
		{
			var soul = (from s in _Souls.UpdateSet() where s.Id == entity_id select s).FirstOrDefault();
			if (soul == null)
				return;

			var propertyInfo = _Protocol.GetMemberMap().GetProperty(property_id);
			if (propertyInfo == null)
				return;
			var binder = NotifierBinder.Create(soul.ObjectInstance, propertyInfo);
			if (binder == null)
				return;
			/*binder.SupplyEvent += (gpi) => _NotifierBindSoul(gpi, entity_id , property_id , handler_id);
			binder.UnsupplyEvent += (gpi) => _NotifierUnbindSoul(gpi, entity_id, property_id, handler_id);
			soul.AttachNotifier(binder);*/

		}

		private void _ErrorDeserialize(string method_name, long return_id, string message)
		{
			

		

			var package = new PackageErrorMethod();
			package.Message = message ;
			package.Method = method_name;
			package.ReturnTarget = return_id;
			_Queue.Push(ServerToClientOpCode.ErrorMethod, package.ToBuffer(_Serializer));
		}

		private void _Bind<TSoul>(TSoul soul, bool return_type, long return_id)
		{
			

			var prevSoul = (from soulInfo in _Souls.UpdateSet()
							where object.ReferenceEquals(soulInfo.ObjectInstance, soul) && soulInfo.ObjectType == typeof(TSoul)
							select soulInfo).SingleOrDefault();

			if(prevSoul == null)
			{
				var newSoul = _NewSoul(soul, typeof(TSoul));

				_LoadSoul(newSoul.InterfaceId, newSoul.Id, return_type);
				_LoadProperty(newSoul);
				_LoadSoulCompile(newSoul.InterfaceId, newSoul.Id, return_id);
			}
		}

		private Soul _NewSoul(object soul, Type soul_type)
		{

		    var map = _Protocol.GetMemberMap();
		    var interfaceId = map.GetInterface(soul_type);
            var newSoul = new Soul(_IdLandlord.Rent(), interfaceId, soul_type, soul);

			

			// property 
			var propertys = soul_type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
			
			for(var i = 0; i < propertys.Length; ++i)
			{
			    var property = propertys[i];			    
			    var id = map.GetProperty(property);            
				
				if (property.PropertyType.GetInterfaces().Any( t=>t == typeof(IDirtyable)))
				{
					var propertyValue = property.GetValue(soul);
					var dirtyable = propertyValue as IDirtyable;		

					var pu = new PropertyUpdater(dirtyable, id); 
					newSoul.AddPropertyUpdater(pu); 					

				}				
			}

			_Souls.Add(newSoul);

			return newSoul;
		}

		

		private void _Unbind(object soul, Type type)
		{
			var soulInfo = (from soul_info in _Souls.UpdateSet()
							where object.ReferenceEquals(soul_info.ObjectInstance, soul) && soul_info.ObjectType == type
							select soul_info).SingleOrDefault();

			if(soulInfo != null)
			{				
				soulInfo.Release();

				

				_UnloadSoul(soulInfo.InterfaceId, soulInfo.Id);
				_Souls.Remove(s => { return s == soulInfo; });
				_IdLandlord.Return(soulInfo.Id);
			}
		}



		

		private Delegate _BuildDelegate(EventInfo info, long entity_id,long handler_id, InvokeEventCallabck invoke_Event)
		{

			var eventCreator = _EventProvider.Find(info);
		    var map = _Protocol.GetMemberMap();
		    var id = map.GetEvent(info);
            return eventCreator.Create(entity_id , id, handler_id, invoke_Event);


			
		}

		public void Update()
		{
			var souls = _Souls.UpdateSet();
			

			foreach (var soul in souls)
			{
				foreach(var pu in soul.PropertyUpdate())
				{
					_LoadProperty(soul.Id, pu.Item1, pu.Item2);					
				}
			}

			lock (_EventFilter)
			{
				foreach(var filter in _EventFilter)
				{
					_Queue.Push(ServerToClientOpCode.InvokeEvent, filter);
				}

				_EventFilter.Clear();
			}
		}

		public void Unbind(long entityId)
		{
			var soul = (from s in _Souls.UpdateSet() where s.Id == entityId select s).FirstOrDefault();
			if(soul != null)
			{
				_Unbind(soul.ObjectInstance, soul.ObjectType);
			}
		}
	}
}
