

using System;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;

using System.Timers;


using Regulus.Utility;

using Regulus.Serialization;


using Timer = System.Timers.Timer;



namespace Regulus.Remoting
{
	/// <summary>
	///     代理器
	/// </summary>
	public interface IAgent : IUpdatable
	{
		/// <summary>
		///     與遠端發生斷線
		///     呼叫Disconnect不會發生此事件
		/// </summary>
		event Action BreakEvent;

		/// <summary>
		///     連線成功事件
		/// </summary>
		event Action ConnectEvent;

		/// <summary>
		///     Ping
		/// </summary>
		long Ping { get; }

		/// <summary>
		///     是否為連線狀態
		/// </summary>
		bool Connected { get; }

		/// <summary>
		///     查詢介面物件通知者
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		INotifier<T> QueryNotifier<T>();

		/// <summary>
		///     連線
		/// </summary>
		/// <param name="ipaddress"></param>
		/// <param name="port"></param>
		/// <returns>如果連線成功會發生OnValue傳回true</returns>
		Value<bool> Connect(string ipaddress, int port);

		/// <summary>
		///     斷線
		/// </summary>
		void Disconnect();

		/// <summary>
		/// 錯誤的方法呼叫
		/// 如果呼叫的方法參數有誤則會回傳此訊息.
		/// 事件參數:
		///     1.方法名稱
		///     2.錯誤訊息
		/// 會發生此訊息通常是因為client與server版本不相容所致.
		/// </summary>
		event Action<string , string> ErrorMethodEvent;
	}

	public class AgentCore
	{
		private static readonly Dictionary<Type, Type> _GhostTypes = new Dictionary<Type, Type>();

		private static readonly Dictionary<string, Type> _Types = new Dictionary<string, Type>();

		private readonly AutoRelease _AutoRelease;

		private readonly Dictionary<string, IProvider> _Providers;

		private readonly ReturnValueQueue _ReturnValueQueue = new ReturnValueQueue();

		private readonly object _Sync = new object();

		private TimeCounter _PingTimeCounter = new TimeCounter();

		private Timer _PingTimer;

		private IGhostRequest _Requester;

		private readonly GPIProvider _GhostProvider;
	    private readonly ISerializer _Serializer;

	    public long Ping { get; private set; }

		public bool Enable { get; private set; }

		public AgentCore(GPIProvider ghost_provider , ISerializer serializer) 
		{
			_GhostProvider = ghost_provider;
		    _Serializer = serializer;
		    _Providers = new Dictionary<string, IProvider>();
            _AutoRelease = new AutoRelease(_Requester , _Serializer);
        }		

		public void Initial(IGhostRequest req)
		{
			_Requester = req;
			_StartPing();

			Enable = true;
		}

		public void Finial()
		{
			Enable = false;
			lock(_Providers)
			{
				foreach(var providerPair in _Providers)
				{
					providerPair.Value.ClearGhosts();
				}
			}

			_EndPing();
		}

		public void OnResponse(ServerToClientOpCode id, byte[] args)
		{
			_OnResponse(id, args);
			_AutoRelease.Update();
		}

		protected void _OnResponse(ServerToClientOpCode id, byte[] args)
		{
			if(id == ServerToClientOpCode.Ping)
			{
				Ping = _PingTimeCounter.Ticks;
				_StartPing();
			}
			else if(id == ServerToClientOpCode.UpdateProperty)
			{
                var data = args.ToPackageData<PackageUpdateProperty>(_Serializer);
                _UpdateProperty(data.EntityId, data.EventName, data.Args);
            }
			else if(id == ServerToClientOpCode.InvokeEvent)
			{
				/*if(args.Count >= 2)
				{
					var EntityId = new Guid(args[0]);
					var EventName = TypeHelper.Deserialize<string>(args[1]);
					var EventParams = (from p in args
									   where p.Key >= 2
									   select p.Value as object).ToArray();


                    var data = args.ToPackageData<PackageInvokeEvent>();
                    _InvokeEvent(data.EntityId, data.EventName, data.EventParams);
				}*/

                var data = args.ToPackageData<PackageInvokeEvent>(_Serializer);
                _InvokeEvent(data.EntityId, data.EventName, data.EventParams);
            }
			else if(id == ServerToClientOpCode.ErrorMethod)
			{
                /*if(args.Count == 3)
				{
					var ReturnTarget = new Guid(args[0]);
					var Method = TypeHelper.Deserialize<string>(args[1]);
					var Message = TypeHelper.Deserialize<string>(args[2]);
					_ErrorReturnValue(ReturnTarget, Method , Message);
				}*/

                var data = args.ToPackageData<PackageErrorMethod>(_Serializer);

                _ErrorReturnValue(data.ReturnTarget, data.Method, data.Message);
            }
			else if(id == ServerToClientOpCode.ReturnValue)
			{
                /*if(args.Count == 2)
				{
					var ReturnTarget = new Guid(args[0]);
					var ReturnValue = args[1];

					_SetReturnValue(ReturnTarget, ReturnValue);
				}*/

                var data = args.ToPackageData<PackageReturnValue>(_Serializer);
                _SetReturnValue(data.ReturnTarget, data.ReturnValue);
            }
			else if(id == ServerToClientOpCode.LoadSoulCompile)
			{
				/*if(args.Count == 3)
				{
					var TypeName = TypeHelper.Deserialize<string>(args[0]);
					var EntityId = new Guid(args[1]);
					var ReturnId = new Guid(args[2]);

					_LoadSoulCompile(TypeName, EntityId, ReturnId);
				}*/

                var data = args.ToPackageData<PackageLoadSoulCompile>(_Serializer);
                _LoadSoulCompile(data.TypeName, data.EntityId, data.ReturnId);
            }
			else if(id == ServerToClientOpCode.LoadSoul)
			{
                /*if(args.Count == 3)
				{
					var TypeName = TypeHelper.Deserialize<string>(args[0]);
					var EntityId = new Guid(args[1]);
					var ReturnType = TypeHelper.Deserialize<bool>(args[2]);

					_LoadSoul(TypeName, EntityId, ReturnType);
				}*/

                var data = args.ToPackageData<PackageLoadSoul>(_Serializer);
                _LoadSoul(data.TypeName, data.EntityId, data.ReturnType);
            }
			else if(id == ServerToClientOpCode.UnloadSoul)
			{
                /*if(args.Count == 2)
				{
					var TypeName = TypeHelper.Deserialize<string>(args[0]);
					var EntityId = new Guid(args[1]);

					_UnloadSoul(TypeName, EntityId);
				}*/

                var data = args.ToPackageData<PackageUnloadSoul>(_Serializer);
                _UnloadSoul(data.TypeName, data.EntityId);
            }
		}

		private void _ErrorReturnValue(Guid return_target, string method, string message)
		{
			_ReturnValueQueue.PopReturnValue(return_target);

			if(ErrorMethodEvent != null)
			{
				ErrorMethodEvent(method , message);
			}
		}

		private void _SetReturnValue(Guid returnTarget, byte[] returnValue)
		{
			var value = _ReturnValueQueue.PopReturnValue(returnTarget);
			if(value != null)
			{
				value.SetValue(returnValue);
			}
		}

		private void _SetReturnValue(Guid return_id, IGhost ghost)
		{
			var value = _ReturnValueQueue.PopReturnValue(return_id);
			if(value != null)
			{
				value.SetValue(ghost);
			}
		}

		private void _LoadSoulCompile(string type_name, Guid entity_id, Guid return_id)
		{
			var provider = _QueryProvider(type_name);
			if(provider != null)
			{
				var ghost = provider.Ready(entity_id);
				_SetReturnValue(return_id, ghost);
			}
		}

		private void _LoadSoul(string type_name, Guid id, bool return_type)
		{
			var provider = _QueryProvider(type_name);
			var ghost = _BuildGhost(AgentCore._GetType(type_name), _Requester, id, return_type);
			provider.Add(ghost);

			if(ghost.IsReturnType())
			{
				_RegisterRelease(ghost);
			}
		}

		private void _RegisterRelease(IGhost ghost)
		{
			_AutoRelease.Register(ghost);
		}

		private void _UnloadSoul(string type_name, Guid id)
		{
			var provider = _QueryProvider(type_name);
			if(provider != null)
			{
				provider.Remove(id);
			}
		}

		private IProvider _QueryProvider(string type_name)
		{
			IProvider provider = null;
			lock(_Providers)
			{
				if(_Providers.TryGetValue(type_name, out provider) == false)
				{
					var type = AgentCore._GetType(type_name);
					if(type != null)
					{
						provider = _BuildProvider(type);
						_Providers.Add(type_name, provider);
					}
				}
			}

			return provider;
		}

		private IProvider _BuildProvider(Type type)
		{
			var providerTemplateType = typeof(TProvider<>);
			var providerType = providerTemplateType.MakeGenericType(type);
			return Activator.CreateInstance(providerType) as IProvider;
		}

		public INotifier<T> QueryProvider<T>()
		{
			return _QueryProvider(typeof(T).FullName) as INotifier<T>;
		}

		private void _UpdateProperty(Guid entity_id, string name, byte[] value)
		{
			var ghost = _FindGhost(entity_id);
			if(ghost != null)
			{
				ghost.OnProperty(name, value);
			}
		}

		private void _InvokeEvent(Guid ghost_id, string eventName, byte[][] eventParams)
		{
			var ghost = _FindGhost(ghost_id);
			if(ghost != null)
			{
				ghost.OnEvent(eventName, eventParams);
			}
		}

		private IGhost _FindGhost(Guid ghost_id)
		{
			lock(_Providers)
			{
				return (from provider in _Providers
						let r = (from g in provider.Value.Ghosts where ghost_id == g.GetID() select g).FirstOrDefault()
						where r != null
						select r).FirstOrDefault();
			}
		}

		protected void _StartPing()
		{
			_EndPing();
			lock(_Sync)
			{
				_PingTimer = new Timer(1000);
				_PingTimer.Enabled = true;
				_PingTimer.AutoReset = true;
				_PingTimer.Elapsed += _PingTimerElapsed;
				_PingTimer.Start();
			}
		}

		private void _PingTimerElapsed(object sender, ElapsedEventArgs e)
		{
			lock(_Sync)
			{
				if(_PingTimer != null)
				{
					_PingTimeCounter = new TimeCounter();
					_Requester.Request(ClientToServerOpCode.Ping, new byte[0]);
				}
			}

			_EndPing();
		}

		protected void _EndPing()
		{
			lock(_Sync)
			{
				if(_PingTimer != null)
				{
					_PingTimer.Stop();
					_PingTimer = null;
				}
			}
		}

		private IGhost _BuildGhost(Type ghost_base_type, IGhostRequest peer, Guid id, bool return_type)
		{
			if(peer == null)
			{
				throw new ArgumentNullException("peer is null");
			}

			var ghostType = _QueryGhostType(ghost_base_type);

			var o = Activator.CreateInstance(ghostType, peer, id, _ReturnValueQueue, return_type , _Serializer);

			return (IGhost)o;
		}

		private Type _QueryGhostType(Type ghostBaseType)
		{
			Type ghostType = null;
			if(AgentCore._GhostTypes.TryGetValue(ghostBaseType, out ghostType))
			{
				return ghostType;
			}

			if (_GhostProvider != null)
				ghostType = _GhostProvider.Find(ghostBaseType);
			

			AgentCore._GhostTypes.Add(ghostBaseType, ghostType);
			return ghostType;
		}

		private static Type _GetType(string type_name)
		{
			lock(AgentCore._Types)
			{
				Type result;
				if(AgentCore._Types.TryGetValue(type_name, out result) == false)
				{
					result = _Find(type_name);
					AgentCore._Types.Add(type_name, result);
				}

				return result;
			}
		}

		private static  Type _Find(string type_name)
		{
			var type = Type.GetType(type_name);
            type = (from t in _GhostTypes.Values where t.Name == type_name select t).FirstOrDefault();
            if (type != null)
                return type;
            if (type == null)
			{
				foreach(var a in AppDomain.CurrentDomain.GetAssemblies())
				{
					type = a.GetType(type_name);
					if(type != null)
					{
						return type;
					}
				}                
                Singleton<Log>.Instance.WriteInfo(string.Format("Fail Type {0}", type_name));
				throw new Exception("找不到gpi " + type_name);
			}

			return type;
		}

		// 被_BuildGhostType參考
		public static void UpdateProperty(string property, string type_name, object instance, byte[] value)
		{
			var type = AgentCore._GetType(type_name);
			if(type != null)
			{
				var field = type.GetField("_" + property, BindingFlags.Instance | BindingFlags.NonPublic);
				if(field != null)
				{
					field.SetValue(instance, TypeHelper.DeserializeObject(field.FieldType, value  ));
				}
			}
		}

		// 被_BuildGhostType參考
		public static void CallEvent(string method, string type_name, object obj, byte[][] args)
		{
			var type = AgentCore._GetType(type_name);

			if(type != null)
			{
				var eventInfos = type.GetField("_" + method, BindingFlags.Instance | BindingFlags.NonPublic);
				var fieldValue = eventInfos.GetValue(obj);
				if(fieldValue is Delegate)
				{
					var fieldValueDelegate = fieldValue as Delegate;
					var parTypes = (from p in fieldValueDelegate.Method.GetParameters()
									select p.ParameterType).ToArray();
					var i = 0;
					var pars = (from a in args select TypeHelper.DeserializeObject(parTypes[i++], a )).ToArray();
					fieldValueDelegate.DynamicInvoke(pars);
				}
			}
		}

		
		

		public event Action<string, string> ErrorMethodEvent;
	}
}
