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

		private readonly Queue<byte[]> _EventFilter = new Queue<byte[]>();

		private readonly IRequestQueue _Peer;

		private readonly IResponseQueue _Queue;

	    private readonly IProtocol _Protocol;

	    private readonly EventProvider _EventProvider;

		private readonly Poller<Soul> _Souls = new Poller<Soul>();

		private readonly Dictionary<Guid, IValue> _WaitValues = new Dictionary<Guid, IValue>();

		private DateTime _UpdatePropertyInterval;
		private readonly ISerializer _Serializer;

		public SoulProvider(IRequestQueue peer, IResponseQueue queue , IProtocol protocol)
		{

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

			_Bind(soul, true, Guid.Empty);
		}

		void IBinder.Bind<TSoul>(TSoul soul)
		{
			if(soul == null)
			{
				throw new ArgumentNullException("soul");
			}

			_Bind(soul, false, Guid.Empty);
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
		
		private void UpdateNormalProperty(Guid entity_id, int property, object val)
		{
			
            
			var package = new PackageUpdateProperty();
		    
            package.EntityId = entity_id;
			package.Property = property;

			
			package.Args = _Serializer.Serialize(val);

			_Queue.Push(ServerToClientOpCode.UpdateProperty, package.ToBuffer(_Serializer));
		}

		private void _InvokeEvent(Guid entity_id, int event_id, object[] args)
		{

		    
            
            var package = new PackageInvokeEvent();
			package.EntityId = entity_id;
			package.Event = event_id;            
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

		private void _ReturnValue(Guid returnId, IValue returnValue)
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

		private void _ReturnSoulValue(Guid return_id, IValue returnValue)
		{
			var soul = returnValue.GetObject();
			var type = returnValue.GetObjectType();
			var prevSoul = (from soulInfo in _Souls.UpdateSet()
							where object.ReferenceEquals(soulInfo.ObjectInstance, soul) && soulInfo.ObjectType == type
							select soulInfo).SingleOrDefault();

			if(prevSoul == null)
			{
				var new_soul = _NewSoul(soul, type);

				_LoadSoul(new_soul.InterfaceId, new_soul.ID, true);

				_LoadProperty(new_soul);

				_LoadSoulCompile(new_soul.InterfaceId, new_soul.ID, return_id);
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
					_LoadProperty(new_soul.ID , id , accessable.Get());
				}
			}
		}

		private void _ReturnDataValue(Guid returnId, IValue returnValue)
		{
			/*var argmants = new Dictionary<byte, byte[]>();
			argmants.Add(0, ReturnId.ToByteArray());
			var value = ReturnValue.GetObject();
			argmants.Add(1, TypeHelper.Serialize(value));*/

			var value = returnValue.GetObject();
			var package = new PackageReturnValue();
			package.ReturnTarget = returnId;
			package.ReturnValue = _Serializer.Serialize(value);
			_Queue.Push(ServerToClientOpCode.ReturnValue, package.ToBuffer(_Serializer));
		}

		private void _LoadSoulCompile(int type_id, Guid id, Guid return_id)
		{
			/*var argmants = new Dictionary<byte, byte[]>();
			argmants.Add(0, TypeHelper.Serialize(type_id));
			argmants.Add(1, id.ToByteArray());
			argmants.Add(2, ReturnId.ToByteArray());*/

			var package = new PackageLoadSoulCompile();
			package.EntityId = id;
			package.ReturnId = return_id;
			package.TypeId = type_id;

			_Queue.Push(ServerToClientOpCode.LoadSoulCompile, package.ToBuffer(_Serializer));
		}
		private void _LoadProperty(Guid id, int property,object val)
		{
			var package = new PackageSetProperty();
			package.EntityId = id;
			package.Property = property;
			package.Value = _Serializer.Serialize(val);
			_Queue.Push(ServerToClientOpCode.SetProperty, package.ToBuffer(_Serializer));
		}
		private void _LoadSoul(int type_id, Guid id, bool return_type)
		{
			var package = new PackageLoadSoul();
			package.TypeId = type_id;
			package.EntityId = id;
			package.ReturnType = return_type;
			_Queue.Push(ServerToClientOpCode.LoadSoul, package.ToBuffer(_Serializer));

			
		}

		private void _UnloadSoul(int type_id, Guid id)
		{			

			var package = new PackageUnloadSoul();
			package.TypeId = type_id;
			package.EntityId = id;
			_Queue.Push(ServerToClientOpCode.UnloadSoul, package.ToBuffer(_Serializer));
		}

        public void SetPropertyDone(Guid entityId, int property)
        {
			var souls = from soul in _Souls.UpdateSet() where soul.ID == entityId select soul;
			var s = souls.FirstOrDefault();
			if(s!= null)
            {
				var updaters  = from updater in s.PropertyUpdaters where updater.PropertyId == property select updater;
				var u = updaters.FirstOrDefault();
				if (u != null)
					u.Reset();
			}
				
		}

        private void _InvokeMethod(Guid entity_id, int method_id, Guid returnId, byte[][] args)
		{
			var soulInfo = (from soul in _Souls.UpdateSet()
							where soul.ID == entity_id
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

		private void _ErrorDeserialize(string method_name, Guid return_id, string message)
		{
			

		

			var package = new PackageErrorMethod();
			package.Message = message ;
			package.Method = method_name;
			package.ReturnTarget = return_id;
			_Queue.Push(ServerToClientOpCode.ErrorMethod, package.ToBuffer(_Serializer));
		}

		private void _Bind<TSoul>(TSoul soul, bool return_type, Guid return_id)
		{
			

			var prevSoul = (from soulInfo in _Souls.UpdateSet()
							where object.ReferenceEquals(soulInfo.ObjectInstance, soul) && soulInfo.ObjectType == typeof(TSoul)
							select soulInfo).SingleOrDefault();

			if(prevSoul == null)
			{
				var newSoul = _NewSoul(soul, typeof(TSoul));

				_LoadSoul(newSoul.InterfaceId, newSoul.ID, return_type);
				_LoadProperty(newSoul);
				_LoadSoulCompile(newSoul.InterfaceId, newSoul.ID, return_id);
			}
		}

		private Soul _NewSoul(object soul, Type soul_type)
		{

		    var map = _Protocol.GetMemberMap();
		    var interfaceId = map.GetInterface(soul_type);
            var new_soul = new Soul
			{
				ID = Guid.NewGuid(), 
                InterfaceId = interfaceId,
                ObjectType = soul_type, 
				ObjectInstance = soul, 
				MethodInfos = soul_type.GetMethods()
			};

			// event				
			var eventInfos = soul_type.GetEvents();
			new_soul.EventHandlers = new List<Soul.EventHandler>();

			foreach(var eventInfo in eventInfos)
			{				
				//var handler = _BuildDelegate(genericArguments, new_soul.ID, eventInfo.Name, _InvokeEvent);

				try
				{
					var handler = _BuildDelegate(eventInfo, new_soul.ID, _InvokeEvent);

					var eh = new Soul.EventHandler();
					eh.EventInfo = eventInfo;
					eh.DelegateObject = handler;
					new_soul.EventHandlers.Add(eh);
					
					var addMethod = eventInfo.GetAddMethod();
					addMethod.Invoke(soul, new[] {handler});
					
				}
				catch (Exception ex)
				{			  
					Regulus.Utility.Log.Instance.WriteDebug(ex.ToString());      
					throw ex;
				}
				
			}

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
					new_soul.PropertyUpdaters.Add(pu) ;					
				}
			}

			_Souls.Add(new_soul);

			return new_soul;
		}

		

		private void _Unbind(object soul, Type type)
		{
			var soulInfo = (from soul_info in _Souls.UpdateSet()
							where object.ReferenceEquals(soul_info.ObjectInstance, soul) && soul_info.ObjectType == type
							select soul_info).SingleOrDefault();

			// var soulInfo = _Souls.CreateInstnace((soul_info) => { return Object.ReferenceEquals(soul_info.ObjectInstance, soul) && soul_info.ObjectType == typeof(TSoul); });
			if(soulInfo != null)
			{
				foreach(var eventHandler in soulInfo.EventHandlers)
				{
					eventHandler.EventInfo.RemoveEventHandler(soulInfo.ObjectInstance, eventHandler.DelegateObject);
				}

				foreach (var pu in soulInfo.PropertyUpdaters)
				{
					pu.Release();
				}

				soulInfo.PropertyUpdaters.Clear();


				_UnloadSoul(soulInfo.InterfaceId, soulInfo.ID);
				_Souls.Remove(s => { return s == soulInfo; });
			}
		}



		

		private Delegate _BuildDelegate(EventInfo info, Guid entity_id, InvokeEventCallabck invoke_Event)
		{

			var eventCreator = _EventProvider.Find(info);
		    var map = _Protocol.GetMemberMap();
		    var id = map.GetEvent(info);
            return eventCreator.Create(entity_id , id, invoke_Event);


			
		}

		public void Update()
		{
			var souls = _Souls.UpdateSet();
			/*var intervalSpan = DateTime.Now - _UpdatePropertyInterval;
			var intervalSeconds = intervalSpan.TotalSeconds;
			if(intervalSeconds > 0.5)
			{
				foreach(var soul in souls)
				{
					soul.ProcessDiffentValues(UpdateNormalProperty);
				}

				_UpdatePropertyInterval = DateTime.Now;
			}*/

			foreach (var soul in souls)
			{
				foreach(var pu in soul.PropertyUpdaters)
				{
					if(pu.Update())
					{
						_LoadProperty(soul.ID , pu.PropertyId , pu.Value);
					}
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

		public void Unbind(Guid entityId)
		{
			var soul = (from s in _Souls.UpdateSet() where s.ID == entityId select s).FirstOrDefault();
			if(soul != null)
			{
				_Unbind(soul.ObjectInstance, soul.ObjectType);
			}
		}
	}
}
