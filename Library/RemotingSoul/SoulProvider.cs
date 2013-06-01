using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samebest.Remoting.Soul
{
	
	public class SoulProvider : IDisposable
	{		
		Samebest.Remoting.Soul.ServerPeer	_Peer;
        Regulus.Remoting.IResponseQueue _Queue;

        public SoulProvider(Samebest.Remoting.Soul.ServerPeer peer, Regulus.Remoting.IResponseQueue queue)
		{
            _Queue = queue;
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
			public Guid ID                                          { get; set; }
			public object							ObjectInstance	{ get; set; }
			public Type								ObjectType		{ get; set; }
            
			public System.Reflection.MethodInfo[]	MethodInfos		{ get; set; }

            public class EventHandler
            {
                public System.Reflection.EventInfo EventInfo;
                public Delegate DelegateObject;
            }
            public System.Collections.Generic.List<EventHandler> EventHandlers { get; set; }


            public class PropertyHandler
            {
                public System.Reflection.PropertyInfo PropertyInfo;
                public object Value;
            
                internal bool UpdateProperty(object val)
                {
                    if ( !(val.Equals(Value) && Value.Equals(val)) )
                    {
                        Value = val;
                        return true;
                    }
                    return false;
                }　
            }
            public PropertyHandler[] PropertyHandlers { get; set; }
            internal void ProcessDiffentValues(Action<Guid, string, object> update_property)
            {
                foreach (var handler in PropertyHandlers)
                {
                    var val = handler.PropertyInfo.GetValue(ObjectInstance, null);                    
                    
                    if(handler.UpdateProperty(val))
                    {
                        update_property(ID, handler.PropertyInfo.Name, val);
                    }                                        
                }
            }
        }
		System.Collections.Generic.List<Soul>	_Souls = new List<Soul>();

        private void _UpdateProperty(Guid entity_id, string name, object val)
        {
            var argmants = new Dictionary<byte, object>();
            argmants.Add(0, entity_id.ToByteArray());
            argmants.Add(1, Samebest.PhotonExtension.TypeHelper.Serializer(name));
            argmants.Add(2, Samebest.PhotonExtension.TypeHelper.Serializer(val));
            
            _Queue.Push((byte)ServerToClientPhotonOpCode.UpdateProperty, argmants);
        }

		void _InvokeEvent(Guid entity_id, string event_name, object[] args)
		{            
            var argmants = new Dictionary<byte, object>();
            argmants.Add(0, entity_id.ToByteArray());
            argmants.Add(1, Samebest.PhotonExtension.TypeHelper.Serializer(event_name));
			byte i = 2;
			foreach (var arg in args)
			{
				argmants.Add( i , Samebest.PhotonExtension.TypeHelper.Serializer(arg) );
				++i;
			}
            _Queue.Push((byte)ServerToClientPhotonOpCode.InvokeEvent, argmants);
		}
        Dictionary<Guid, IValue> _WaitValues = new Dictionary<Guid, IValue>();
        private void _ReturnValue(Guid returnId, IValue returnValue)
        {
            _WaitValues.Add(returnId, returnValue);
            returnValue.QueryValue(new Action<object>((obj) =>
            {
                var argmants = new Dictionary<byte, object>();
                argmants.Add(0, returnId.ToByteArray());
                object value = returnValue.GetObject();
                argmants.Add(1, PhotonExtension.TypeHelper.Serializer(value));
                _Queue.Push((byte)ServerToClientPhotonOpCode.ReturnValue, argmants);

                _WaitValues.Remove(returnId);
            }));            
        }
        private void _LoadSoul(string type_name, Guid id)
        {         
            var argmants = new Dictionary<byte, object>();
            argmants.Add(0, Samebest.PhotonExtension.TypeHelper.Serializer(type_name));
            argmants.Add(1, id.ToByteArray());
            _Queue.Push((byte)ServerToClientPhotonOpCode.LoadSoul, argmants);
        }

        private void _UnloadSoul(string type_name, Guid id)
        {
            var argmants = new Dictionary<byte, object>();
            argmants.Add(0, Samebest.PhotonExtension.TypeHelper.Serializer(type_name));
            argmants.Add(1, id.ToByteArray());
            _Queue.Push((byte)ServerToClientPhotonOpCode.UnloadSoul, argmants);
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
		
		public void Bind<TSoul>(TSoul soul)
		{
			var prevSoul = _Souls.Find((soulInfo) => { return soulInfo.ObjectType == typeof(TSoul); });
			if (prevSoul == null)
			{
				var new_soul = new Soul() { ID = Guid.NewGuid(), ObjectType = typeof(TSoul), ObjectInstance = soul, MethodInfos = typeof(TSoul).GetMethods()};

				Type soulType = typeof(TSoul);

                
                // event				
                var eventInfos = soulType.GetEvents();
                new_soul.EventHandlers = new List<Soul.EventHandler>();
				foreach(var eventInfo in eventInfos)
				{					
					var genericArguments =  eventInfo.EventHandlerType.GetGenericArguments();
					Delegate handler = _BuildDelegate(genericArguments, new_soul.ID , eventInfo.Name);
                    Soul.EventHandler eh = new Soul.EventHandler();
                    eh.EventInfo = eventInfo;
                    eh.DelegateObject = handler;
                    new_soul.EventHandlers.Add( eh );
					eventInfo.AddEventHandler(soul , handler);
				}

                // property 
                var propertys = soulType.GetProperties( System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public );
                new_soul.PropertyHandlers = new Soul.PropertyHandler[propertys.Length];
                for (int i = 0; i < propertys.Length; ++i )
                {
                    new_soul.PropertyHandlers[i] = new Soul.PropertyHandler();
                    new_soul.PropertyHandlers[i].PropertyInfo = propertys[i];
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

		public void Unbind<TSoul>(TSoul soul)
		{
			var soulInfo = _Souls.Find((soul_info) => { return soul_info.ObjectInstance  == soul as object; });
			if (soulInfo != null)
			{
				foreach (var eventHandler in soulInfo.EventHandlers)
				{
                    eventHandler.EventInfo.RemoveEventHandler(soulInfo.ObjectInstance, eventHandler.DelegateObject );
				}
				_UnloadSoul(soulInfo.ObjectType.FullName , soulInfo.ID);
				_Souls.Remove(soulInfo);
				
			}
		}

		public void Dispose()
		{
			_Peer.InvokeMethodEvent -= _InvokeMethod;

		}


        System.DateTime _UpdatePropertyInterval;
        internal void Update()
        {
            _Peer.Update();
            if ((System.DateTime.Now - _UpdatePropertyInterval).TotalSeconds > 1)
            {
                foreach (var soul in _Souls)
                {
                    soul.ProcessDiffentValues(_UpdateProperty);
                }
                _UpdatePropertyInterval = System.DateTime.Now;
            }
            
        }

        
    }
}
