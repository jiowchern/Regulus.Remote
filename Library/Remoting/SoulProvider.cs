using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Regulus.Utility;

namespace Regulus.Remoting
{
	public class SoulProvider : IDisposable, ISoulBinder
	{
		private class Soul
		{


			public class EventHandler
			{
				public Delegate DelegateObject;

				public EventInfo EventInfo;
			}

			public class PropertyHandler
			{
				public PropertyInfo PropertyInfo;

				public object Value;

				internal bool UpdateProperty(object val)
				{
					if(!ValueHelper.DeepEqual(Value, val))
					{
						Value = ValueHelper.DeepCopy(val);
						return true;
					}

					return false;
				}
			}

			public Guid ID { get; set; }

			public object ObjectInstance { get; set; }

			public Type ObjectType { get; set; }

			public MethodInfo[] MethodInfos { get; set; }

			public List<EventHandler> EventHandlers { get; set; }

			public PropertyHandler[] PropertyHandlers { get; set; }

			internal void ProcessDiffentValues(Action<Guid, string, object> update_property)
			{
				foreach(var handler in PropertyHandlers)
				{
					var val = handler.PropertyInfo.GetValue(ObjectInstance, null);

					if(handler.UpdateProperty(val))
					{
						if(update_property != null)
						{
							update_property(ID, handler.PropertyInfo.Name, val);
						}
					}
				}
			}
		}

		private readonly Queue<byte[]> _EventFilter = new Queue<byte[]>();

		private readonly IRequestQueue _Peer;

		private readonly IResponseQueue _Queue;

		private readonly Poller<Soul> _Souls = new Poller<Soul>();

		private readonly Dictionary<Guid, IValue> _WaitValues = new Dictionary<Guid, IValue>();

		private DateTime _UpdatePropertyInterval;

		public SoulProvider(IRequestQueue peer, IResponseQueue queue)
		{
			_Queue = queue;
			_Peer = peer;
			_Peer.InvokeMethodEvent += _InvokeMethod;
		}

		public void Dispose()
		{
			_Peer.InvokeMethodEvent -= _InvokeMethod;
		}

		void ISoulBinder.Return<TSoul>(TSoul soul)
		{
			if(soul == null)
			{
				throw new ArgumentNullException("soul");
			}

			_Bind(soul, true, Guid.Empty);
		}

		void ISoulBinder.Bind<TSoul>(TSoul soul)
		{
			if(soul == null)
			{
				throw new ArgumentNullException("soul");
			}

			_Bind(soul, false, Guid.Empty);
		}

		void ISoulBinder.Unbind<TSoul>(TSoul soul)
		{
			if(soul == null)
			{
				throw new ArgumentNullException("soul");
			}

			_Unbind(soul, typeof(TSoul));
		}

		event Action ISoulBinder.BreakEvent
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

		// System.Collections.Generic.List<Soul>	_Souls = new List<Soul>();
		private void _UpdateProperty(Guid entity_id, string name, object val)
		{
			/*var argmants = new Dictionary<byte, byte[]>();
			argmants.Add(0, EntityId.ToByteArray());
			argmants.Add(1, TypeHelper.Serializer(name));
			argmants.Add(2, TypeHelper.Serializer(val));*/

            var package = new PackageUpdateProperty();
		    package.EntityId = entity_id;
		    package.EventName = name;
            package.Args = TypeHelper.Serializer(val);

            _Queue.Push(ServerToClientOpCode.UpdateProperty, package.ToBuffer());
		}

		private void _InvokeEvent(Guid entity_id, string event_name, object[] args)
		{
			/*var argmants = new Dictionary<byte, byte[]>();
			argmants.Add(0, EntityId.ToByteArray());
			argmants.Add(1, TypeHelper.Serializer(event_name));
			byte i = 2;
			foreach(var arg in args)
			{
				argmants.Add(i, TypeHelper.Serializer(arg));
				++i;
			}*/


            var package = new PackageInvokeEvent();
		    package.EntityId = entity_id;
		    package.EventName = event_name;            
            package.EventParams = (from a in args select TypeHelper.Serializer(a)).ToArray();
            _InvokeEvent(package.ToBuffer());
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

				_LoadSoul(type.FullName, new_soul.ID, true);
				new_soul.ProcessDiffentValues(_UpdateProperty);
				_LoadSoulCompile(type.FullName, new_soul.ID, return_id);
			}
		}

		private void _ReturnDataValue(Guid returnId, IValue returnValue)
		{
            /*var argmants = new Dictionary<byte, byte[]>();
			argmants.Add(0, ReturnId.ToByteArray());
			var value = ReturnValue.GetObject();
			argmants.Add(1, TypeHelper.Serializer(value));*/

            var value = returnValue.GetObject();
            var package = new PackageReturnValue();
		    package.ReturnTarget = returnId;
		    package.ReturnValue = TypeHelper.Serializer(value);
            _Queue.Push(ServerToClientOpCode.ReturnValue, package.ToBuffer());
		}

		private void _LoadSoulCompile(string type_name, Guid id, Guid return_id)
		{
			/*var argmants = new Dictionary<byte, byte[]>();
			argmants.Add(0, TypeHelper.Serializer(type_name));
			argmants.Add(1, id.ToByteArray());
			argmants.Add(2, ReturnId.ToByteArray());*/

            var package = new PackageLoadSoulCompile();
		    package.EntityId = id;
		    package.ReturnId = return_id;
		    package.TypeName = type_name;

            _Queue.Push(ServerToClientOpCode.LoadSoulCompile, package.ToBuffer());
		}

		private void _LoadSoul(string type_name, Guid id, bool return_type)
		{
			/*var argmants = new Dictionary<byte, byte[]>();
			argmants.Add(0, TypeHelper.Serializer(type_name));
			argmants.Add(1, id.ToByteArray());
			argmants.Add(2, TypeHelper.Serializer(return_type));*/


            var package = new PackageLoadSoul();
		    package.TypeName = type_name;
		    package.EntityId = id;
		    package.ReturnType = return_type;
            _Queue.Push(ServerToClientOpCode.LoadSoul, package.ToBuffer());
		}

		private void _UnloadSoul(string type_name, Guid id)
		{
			/*var argmants = new Dictionary<byte, byte[]>();
			argmants.Add(0, TypeHelper.Serializer(type_name));
			argmants.Add(1, id.ToByteArray());*/

            var package = new PackageUnloadSoul();
		    package.TypeName = type_name;
		    package.EntityId = id;
            _Queue.Push(ServerToClientOpCode.UnloadSoul, package.ToBuffer());
		}

		private void _InvokeMethod(Guid entity_id, string method_name, Guid returnId, byte[][] args)
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
				var methodInfo =
					(from m in soulInfo.MethodInfos where m.Name == method_name && m.GetParameters().Count() == args.Count() select m)
						.FirstOrDefault();
				if(methodInfo != null)
				{
					var paramerInfos = methodInfo.GetParameters();

					try
					{
						var i = 0;
						var argObjects = from pi in paramerInfos
										 let arg = args[i++]
										 select TypeHelper.DeserializeObject(pi.ParameterType, arg);

						var returnValue = methodInfo.Invoke(soulInfo.ObjectInstance, argObjects.ToArray());
						if(returnValue != null)
						{
							_ReturnValue(returnId, returnValue as IValue);
						}
					}
					catch(DeserializeException deserialize_exception)
					{
						var message  =  deserialize_exception.Base.ToString();                        
						_ErrorDeserialize(method_name, returnId , message);
					}
					catch(Exception e)
					{
						Log.Instance.WriteDebug(e.ToString());
						_ErrorDeserialize(method_name, returnId, e.Message);
					}
					
				}
			}
		}

		private void _ErrorDeserialize(string method_name, Guid return_id, string message)
		{
			

			/*var argmants = new Dictionary<byte, byte[]>();
			argmants.Add(0, ReturnId.ToByteArray());            
			argmants.Add(1, TypeHelper.Serializer(method_name));
			argmants.Add(2, TypeHelper.Serializer(Message));*/

		    var package = new PackageErrorMethod();
		    package.Message = message ;
		    package.Method = method_name;
		    package.ReturnTarget = return_id;
            _Queue.Push(ServerToClientOpCode.ErrorMethod, package.ToBuffer());
		}

		private void _Bind<TSoul>(TSoul soul, bool return_type, Guid return_id)
		{
			var type = typeof(TSoul);

			var prevSoul = (from soulInfo in _Souls.UpdateSet()
							where object.ReferenceEquals(soulInfo.ObjectInstance, soul) && soulInfo.ObjectType == typeof(TSoul)
							select soulInfo).SingleOrDefault();

			if(prevSoul == null)
			{
				var new_soul = _NewSoul(soul, typeof(TSoul));

				_LoadSoul(type.FullName, new_soul.ID, return_type);
				new_soul.ProcessDiffentValues(_UpdateProperty);
				_LoadSoulCompile(type.FullName, new_soul.ID, return_id);
			}
		}

		private Soul _NewSoul(object soul, Type soulType)
		{
			// var bindChecker = new BindGuard(soulType);
			var new_soul = new Soul
			{
				ID = Guid.NewGuid(), 
				ObjectType = soulType, 
				ObjectInstance = soul, 
				MethodInfos = soulType.GetMethods()
			};

			// event				
			var eventInfos = soulType.GetEvents();
			new_soul.EventHandlers = new List<Soul.EventHandler>();

			foreach(var eventInfo in eventInfos)
			{
				var genericArguments = eventInfo.EventHandlerType.GetGenericArguments();
				var handler = _BuildDelegate(genericArguments, new_soul.ID, eventInfo.Name);

				var eh = new Soul.EventHandler();
				eh.EventInfo = eventInfo;
				eh.DelegateObject = handler;
				new_soul.EventHandlers.Add(eh);

				eventInfo.AddEventHandler(soul, handler);
			}

			// property 
			var propertys = soulType.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
			new_soul.PropertyHandlers = new Soul.PropertyHandler[propertys.Length];
			for(var i = 0; i < propertys.Length; ++i)
			{
				new_soul.PropertyHandlers[i] = new Soul.PropertyHandler();
				new_soul.PropertyHandlers[i].PropertyInfo = propertys[i];
			}

			_Souls.Add(new_soul);

			return new_soul;
		}

		private void _Unbind(object soul, Type type)
		{
			var soulInfo = (from soul_info in _Souls.UpdateSet()
							where object.ReferenceEquals(soul_info.ObjectInstance, soul) && soul_info.ObjectType == type
							select soul_info).SingleOrDefault();

			// var soulInfo = _Souls.Find((soul_info) => { return Object.ReferenceEquals(soul_info.ObjectInstance, soul) && soul_info.ObjectType == typeof(TSoul); });
			if(soulInfo != null)
			{
				foreach(var eventHandler in soulInfo.EventHandlers)
				{
					eventHandler.EventInfo.RemoveEventHandler(soulInfo.ObjectInstance, eventHandler.DelegateObject);
				}

				_UnloadSoul(soulInfo.ObjectType.FullName, soulInfo.ID);
				_Souls.Remove(s => { return s == soulInfo; });
			}
		}

		private Delegate _BuildDelegate(Type[] generic_arguments, Guid entity_id, string event_name)
		{
			Type closureType = null;

			Type delegateType = null;
			MethodInfo run = null;
			Type[] closureTypes =
			{
				typeof(GenericEventClosure)
				, 
				typeof(GenericEventClosure<>)
				, 
				typeof(GenericEventClosure<,>)
				, 
				typeof(GenericEventClosure<,,>)
				, 
				typeof(GenericEventClosure<,,,>)
				, 
				typeof(GenericEventClosure<,,,,>)
			};

			closureType = closureTypes[generic_arguments.Length];
			if(generic_arguments.Length != 0)
			{
				closureType = closureType.MakeGenericType(generic_arguments);
				var getDelegateTypeMethod = closureType.GetMethod("GetDelegateType", BindingFlags.Static | BindingFlags.Public);
				delegateType = (Type)getDelegateTypeMethod.Invoke(null, null);
			}
			else
			{
				delegateType = GenericEventClosure.GetDelegateType();
			}

			run = closureType.GetMethod("Run");
			var closureInstance = Activator.CreateInstance(
				closureType, 
				entity_id, 
				event_name, 
				new Action<Guid, string, object[]>(_InvokeEvent));

			return Delegate.CreateDelegate(delegateType, closureInstance, run);
		}

		public void Update()
		{
			var souls = _Souls.UpdateSet();
			var intervalSpan = DateTime.Now - _UpdatePropertyInterval;
			var intervalSeconds = intervalSpan.TotalSeconds;
			if(intervalSeconds > 0.5)
			{
				foreach(var soul in souls)
				{
					soul.ProcessDiffentValues(_UpdateProperty);
				}

				_UpdatePropertyInterval = DateTime.Now;
			}

			lock(_EventFilter)
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
