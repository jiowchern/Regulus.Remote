#region Test_Region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Timers;


using Regulus.Utility;


using Timer = System.Timers.Timer;

#endregion

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

		private readonly IGhostProvider _GhostProvider;

		public long Ping { get; private set; }

		public bool Enable { get; private set; }

		public AgentCore(IGhostProvider ghost_provider) : this()
		{
			_GhostProvider = ghost_provider;
		}
		public AgentCore()
		{
			_Providers = new Dictionary<string, IProvider>();

			_AutoRelease = new AutoRelease(_Requester);
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
                var data = args.ToPackageData<PackageUpdateProperty>();
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

                var data = args.ToPackageData<PackageInvokeEvent>();
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

                var data = args.ToPackageData<PackageErrorMethod>();

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

                var data = args.ToPackageData<PackageReturnValue>();
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

                var data = args.ToPackageData<PackageLoadSoulCompile>();
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

                var data = args.ToPackageData<PackageLoadSoul>();
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

                var data = args.ToPackageData<PackageUnloadSoul>();
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

		private IGhost _BuildGhost(Type ghostBaseType, IGhostRequest peer, Guid id, bool return_type)
		{
			if(peer == null)
			{
				throw new ArgumentNullException("peer is null");
			}

			var ghostType = _QueryGhostType(ghostBaseType);

			var o = Activator.CreateInstance(ghostType, peer, id, _ReturnValueQueue, return_type);

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

			if(ghostType == null)
				ghostType = AgentCore._BuildGhostType(ghostBaseType);

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

		private static Type _BuildGhostType(Type ghostBaseType)
		{
			// 反射機制
			var baseType = ghostBaseType;

			// 產生class的組態
			var asmName = new AssemblyName("RegulusRemotingGhost." + baseType + "Assembly");

			// 從目前的domain裡即時產生一個組態                                    
			var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);            

			// 產生一個模組
			var module = assembly.DefineDynamicModule("RegulusRemotingGhost." + baseType + "Module");

			// 產生一個class or struct
			// 這裡是用class
			var typeName = "C" + baseType;
			var type = module.DefineType(
				typeName, 
				TypeAttributes.Class | TypeAttributes.Sealed, 
				typeof(object), 
				new[]
				{
					baseType, 
					typeof(IGhost)
				});

			#region build constructor

			// 產生建構子，有一個參數 tpeer
			var c = type.DefineConstructor(
				MethodAttributes.Public, 
				CallingConventions.Standard, 
				new[]
				{
					typeof(IGhostRequest), 
					typeof(Guid), 
					typeof(ReturnValueQueue), 
					typeof(bool)
				});

			// 產生field，一個欄位
			var returnTypeField = type.DefineField("_ReturnType", typeof(bool), FieldAttributes.Private);
			var peerField = type.DefineField("_Peer", typeof(IGhostRequest), FieldAttributes.Private);
			var idField = type.DefineField("_ID", typeof(Guid), FieldAttributes.Private);
			var rvqField = type.DefineField("_ReturnValueQueue", typeof(ReturnValueQueue), FieldAttributes.Private);

			// 取得中介語言的介面產生器
			var cil = c.GetILGenerator();

			// emit為c#中介語言的寫入方法工具
			// opcode為中介語的opcodes
			cil.Emit(OpCodes.Ldarg_0); // this 指標
			cil.Emit(OpCodes.Ldarg_1); // functioin第1個參數的值
			cil.Emit(OpCodes.Stfld, peerField); // 下設定指令

			cil.Emit(OpCodes.Ldarg_0); // this 指標
			cil.Emit(OpCodes.Ldarg_2); // functioin第2個參數的值
			cil.Emit(OpCodes.Stfld, idField); // 下設定指令

			cil.Emit(OpCodes.Ldarg_0); // this 指標
			cil.Emit(OpCodes.Ldarg_3); // functioin第3個參數的值
			cil.Emit(OpCodes.Stfld, rvqField); // 下設定指令

			cil.Emit(OpCodes.Ldarg_0); // this 指標
			cil.Emit(OpCodes.Ldarg_S, 4); // functioin第3個參數的值
			cil.Emit(OpCodes.Stfld, returnTypeField); // 下設定指令

			var objectType = typeof(object);
			var objectTypeConstructor = objectType.GetConstructor(new Type[0]);

			cil.Emit(OpCodes.Ldarg_0); // this 指標
			cil.Emit(OpCodes.Call, objectTypeConstructor);

			cil.Emit(OpCodes.Ret); // return 出去

			#endregion

			#region build IGhostEventListener Method

			var methodGetIDInfo = typeof(IGhost).GetMethod("GetID");
			if(methodGetIDInfo != null)
			{
				var argTypes =
					(from parameter in methodGetIDInfo.GetParameters() orderby parameter.Position select parameter.ParameterType)
						.ToArray();
				var methodBuilder = type.DefineMethod(
					methodGetIDInfo.Name, 
					methodGetIDInfo.Attributes & ~MethodAttributes.Abstract, 
					typeof(Guid), 
					argTypes);
				var methidIL = methodBuilder.GetILGenerator();
				methidIL.Emit(OpCodes.Nop);
				methidIL.Emit(OpCodes.Ldarg_0); // this
				methidIL.Emit(OpCodes.Ldfld, idField); // id
				methidIL.Emit(OpCodes.Ret);

				type.DefineMethodOverride(methodBuilder, methodGetIDInfo);
			}

			var methodIsReturnTypeInfo = typeof(IGhost).GetMethod("IsReturnType");
			if(methodIsReturnTypeInfo != null)
			{
				var argTypes =
					(from parameter in methodIsReturnTypeInfo.GetParameters()
					 orderby parameter.Position
					 select parameter.ParameterType)
						.ToArray();
				var methodBuilder = type.DefineMethod(
					methodIsReturnTypeInfo.Name, 
					methodIsReturnTypeInfo.Attributes & ~MethodAttributes.Abstract, 
					typeof(bool), 
					argTypes);
				var methidIL = methodBuilder.GetILGenerator();
				methidIL.Emit(OpCodes.Nop);
				methidIL.Emit(OpCodes.Ldarg_0); // this
				methidIL.Emit(OpCodes.Ldfld, returnTypeField); // id
				methidIL.Emit(OpCodes.Ret);

				type.DefineMethodOverride(methodBuilder, methodIsReturnTypeInfo);
			}

			var propertyInfos = baseType.GetProperties();
			foreach(var propertyInfo in propertyInfos)
			{
				var propertyType = propertyInfo.PropertyType;
				var field = type.DefineField("_" + propertyInfo.Name, propertyType, FieldAttributes.Private);
				var property = type.DefineProperty(propertyInfo.Name, PropertyAttributes.HasDefault, propertyType, null);

				if(propertyInfo.CanRead)
				{
					var baseMethod = propertyInfo.GetGetMethod();

					var method = type.DefineMethod(
						"get_" + propertyInfo.Name, 
						baseMethod.Attributes & ~MethodAttributes.Abstract, 
						propertyType, 
						Type.EmptyTypes);
					var methodIL = method.GetILGenerator();
					methodIL.Emit(OpCodes.Ldarg_0);
					methodIL.Emit(OpCodes.Ldfld, field);
					methodIL.Emit(OpCodes.Ret);
					property.SetGetMethod(method);
					type.DefineMethodOverride(method, baseMethod);
				}

				if(propertyInfo.CanWrite)
				{
					var baseMethod = propertyInfo.GetGetMethod();
					var method = type.DefineMethod(
						"set_" + propertyInfo.Name, 
						baseMethod.Attributes & ~MethodAttributes.Abstract, 
						null, 
						new[]
						{
							propertyType
						});
					var methodIL = method.GetILGenerator();
					methodIL.Emit(OpCodes.Ldarg_0);
					methodIL.Emit(OpCodes.Ldarg_1);
					methodIL.Emit(OpCodes.Stfld, field);
					methodIL.Emit(OpCodes.Ret);
					property.SetSetMethod(method);
					type.DefineMethodOverride(method, baseMethod);
				}
			}

			var methodOnProperty = typeof(IGhost).GetMethod("OnProperty");
			if(methodOnProperty != null)
			{
				var argTypes =
					(from parameter in methodOnProperty.GetParameters() orderby parameter.Position select parameter.ParameterType)
						.ToArray();
				var method = type.DefineMethod(
					methodOnProperty.Name, 
					methodOnProperty.Attributes & ~MethodAttributes.Abstract, 
					methodOnProperty.ReturnType, 
					argTypes);
				var methodIL = method.GetILGenerator();

				methodIL.Emit(OpCodes.Ldarg_1);
				methodIL.Emit(OpCodes.Ldstr, typeName);
				methodIL.Emit(OpCodes.Ldarg_0);
				methodIL.Emit(OpCodes.Ldarg_2);
				methodIL.Emit(
					OpCodes.Call, 
					typeof(AgentCore).GetMethod("UpdateProperty", BindingFlags.Public | BindingFlags.Static));
				methodIL.Emit(OpCodes.Nop);
				methodIL.Emit(OpCodes.Ret);

				type.DefineMethodOverride(method, methodOnProperty);
			}

			var methodOnEventInfo = typeof(IGhost).GetMethod("OnEvent");
			if(methodOnEventInfo != null)
			{
				var argTypes =
					(from parameter in methodOnEventInfo.GetParameters() orderby parameter.Position select parameter.ParameterType)
						.ToArray();
				var methodBuilder = type.DefineMethod(
					methodOnEventInfo.Name, 
					methodOnEventInfo.Attributes & ~MethodAttributes.Abstract, 
					null, 
					argTypes);
				var methidIL = methodBuilder.GetILGenerator();

				methidIL.Emit(OpCodes.Nop);
				methidIL.Emit(OpCodes.Ldarg_1);
				methidIL.Emit(OpCodes.Ldstr, typeName);
				methidIL.Emit(OpCodes.Ldarg_0);
				methidIL.Emit(OpCodes.Ldarg_2);
				methidIL.Emit(OpCodes.Call, typeof(AgentCore).GetMethod("CallEvent", BindingFlags.Public | BindingFlags.Static));
				methidIL.Emit(OpCodes.Nop);
				methidIL.Emit(OpCodes.Ret);

				type.DefineMethodOverride(methodBuilder, methodOnEventInfo);
			}

			#endregion

			#region build Method

			var methods = baseType.GetMethods();

			foreach(var m in methods)
			{
				if(m.IsSpecialName)
				{
					continue;
				}

				// 取出介面的fun，去除Abstract屬性
				var attribute = m.Attributes & ~MethodAttributes.Abstract;

				// 取出function的參數
				var pars = m.GetParameters();

				// 取出參數的型別，用types array 裝
				var types = new Type[pars.Length];
				var i = 0;
				foreach(var p in pars)
				{
					types[i++] = p.ParameterType;
				}

				// 產生一個function
				var method = type.DefineMethod(m.Name, attribute, m.ReturnType, types);

				// 取得中介語言的介面產生器
				var il = method.GetILGenerator();

				var byteArrayType = typeof(byte[]);
				var varGuidByteArray = il.DeclareLocal(byteArrayType);
				var varMethodNameByteArray = il.DeclareLocal(byteArrayType);

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, idField);
				var guidToByteArrayMethod = typeof(TypeHelper).GetMethod(
					"GuidToByteArray", 
					BindingFlags.Public | BindingFlags.Static);
				il.Emit(OpCodes.Call, guidToByteArrayMethod);
				il.Emit(OpCodes.Stloc, varGuidByteArray);

				il.Emit(OpCodes.Ldstr, m.Name);
				var stringToByteArrayMethod = typeof(TypeHelper).GetMethod(
					"StringToByteArray", 
					BindingFlags.Public | BindingFlags.Static);
				il.Emit(OpCodes.Call, stringToByteArrayMethod);
				il.Emit(OpCodes.Stloc, varMethodNameByteArray);

				// 取出type物件
				var dictionaryType = typeof(Dictionary<byte, byte[]>);

				// 宣告函式的local變數
				var varDict = il.DeclareLocal(dictionaryType);

				// new出指定物的建構子
				il.Emit(OpCodes.Newobj, dictionaryType.GetConstructor(Type.EmptyTypes));

				// 設定local變數
				il.Emit(OpCodes.Stloc, varDict);

				// add id
				il.Emit(OpCodes.Ldloc, varDict);
				il.Emit(OpCodes.Ldc_I4, 0);
				il.Emit(OpCodes.Ldloc, varGuidByteArray);
				il.Emit(OpCodes.Call, varDict.LocalType.GetMethod("Add"));

				// add Method name
				il.Emit(OpCodes.Ldloc, varDict);
				il.Emit(OpCodes.Ldc_I4, 1);
				il.Emit(OpCodes.Ldloc, varMethodNameByteArray);
				il.Emit(OpCodes.Call, varDict.LocalType.GetMethod("Add"));

				// push return info
				var valueOriType = typeof(Value<>);

				LocalBuilder varValueObject = null;

				if(valueOriType.Name == m.ReturnType.Name && valueOriType.Namespace == m.ReturnType.Namespace)
				{
					var argTypes = m.ReturnType.GetGenericArguments();
					var valueType = valueOriType.MakeGenericType(argTypes[0]);

					il.Emit(OpCodes.Newobj, valueType.GetConstructor(Type.EmptyTypes));
					var varValue = il.DeclareLocal(valueType);
					varValueObject = il.DeclareLocal(valueType);
					il.Emit(OpCodes.Stloc, varValue);

					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, rvqField);
					il.Emit(OpCodes.Ldloc, varValue);
					il.Emit(OpCodes.Call, rvqField.FieldType.GetMethod("PushReturnValue"));
					var varRVQId = il.DeclareLocal(typeof(Guid));
					il.Emit(OpCodes.Stloc, varRVQId);

					il.Emit(OpCodes.Ldloc, varRVQId);
					il.Emit(OpCodes.Call, typeof(TypeHelper).GetMethod("GuidToByteArray", BindingFlags.Public | BindingFlags.Static));
					var varRVQIdByteArray = il.DeclareLocal(typeof(byte[]));
					il.Emit(OpCodes.Stloc, varRVQIdByteArray);

					il.Emit(OpCodes.Ldloc, varDict);
					il.Emit(OpCodes.Ldc_I4, 2);
					il.Emit(OpCodes.Ldloc, varRVQIdByteArray);
					il.Emit(OpCodes.Call, varDict.LocalType.GetMethod("Add"));

					il.Emit(OpCodes.Ldloc, varValue);
					il.Emit(OpCodes.Stloc, varValueObject);
				}

				for(var paramIndex = 0; paramIndex < pars.Length; paramIndex++)
				{
					// 建立local變數，型別byte
					var varBuffer = il.DeclareLocal(typeof(byte[]));

					// 將0  有符號的整數存到stacK
					il.Emit(OpCodes.Ldc_I4_S, 0);

					// new出byte的array
					il.Emit(OpCodes.Newarr, typeof(byte));

					// 將array的值設定到varBuffer
					il.Emit(OpCodes.Stloc, varBuffer);

					// 讀取參數的值 從0開始
					il.Emit(OpCodes.Ldarg, 1 + paramIndex);

					// 將參數真正的型別轉出來
					// il.Emit(OpCodes.Box, types[paramIndex]);

					// 使用TypeHelper類別裡的Serializer函式 屬性為Public Static..
					var serializer =
						typeof(TypeHelper).GetMethod("Serializer", BindingFlags.Public | BindingFlags.Static)
										  .MakeGenericMethod(types[paramIndex]);

					// 指定呼叫函式的多載，因為沒有多載，所以填null
					il.EmitCall(OpCodes.Call, serializer, null);

					// byte array 存到 varBuffer
					il.Emit(OpCodes.Stloc, varBuffer);

					il.Emit(OpCodes.Ldloc, varDict);
					il.Emit(OpCodes.Ldc_I4, 3 + paramIndex);
					il.Emit(OpCodes.Ldloc, varBuffer);

					il.Emit(OpCodes.Call, varDict.LocalType.GetMethod("Add"));
				}

				AgentCore.TestEmitYield(il);

				// 填函式要的資料
				il.Emit(OpCodes.Ldarg_0); // this
				il.Emit(OpCodes.Ldfld, peerField); // peer
				il.Emit(OpCodes.Ldc_I4, (int)ClientToServerOpCode.CallMethod); // opcode 
				il.Emit(OpCodes.Ldloc, varDict);

				// 指定呼叫函式的多載
				il.Emit(
					OpCodes.Callvirt, 
					peerField.FieldType.GetMethod(
						"Request", 
						new[]
						{
							typeof(byte), 
							dictionaryType
						}));

				AgentCore.TestEmitYield(il);

				if(valueOriType.Name == m.ReturnType.Name && valueOriType.Namespace == m.ReturnType.Namespace)
				{
					il.Emit(OpCodes.Ldloc, varValueObject);
				}

				AgentCore.TestEmitYield(il);

				// return;
				il.Emit(OpCodes.Ret);

				// 指定覆寫的fun
				type.DefineMethodOverride(method, m);
			}

			#endregion

			#region build event

			var eventInfos = baseType.GetEvents();
			foreach(var eventInfo in eventInfos)
			{
				var eventName = "_" + eventInfo.Name;
				var eventHandleType = eventInfo.EventHandlerType;
				var eventFieldBuilder = type.DefineField(eventName, eventHandleType, FieldAttributes.FamORAssem);
				var eventBuilder = type.DefineEvent(eventInfo.Name, EventAttributes.None, eventHandleType);

				#region add event

				var addEventBuilder = type.DefineMethod(
					"add_" + eventInfo.Name, 
					eventInfo.GetAddMethod().Attributes & ~MethodAttributes.Abstract, 
					null, 
					new[]
					{
						eventHandleType
					});
				var addEventIL = addEventBuilder.GetILGenerator();
				addEventIL.Emit(OpCodes.Ldarg_0);
				addEventIL.Emit(OpCodes.Ldarg_0);
				addEventIL.Emit(OpCodes.Ldfld, eventFieldBuilder);
				addEventIL.Emit(OpCodes.Ldarg_1);
				addEventIL.Emit(
					OpCodes.Call, 
					typeof(Delegate).GetMethod(
						"Combine", 
						new[]
						{
							typeof(Delegate), 
							typeof(Delegate)
						}));

				addEventIL.Emit(OpCodes.Castclass, eventHandleType);
				addEventIL.Emit(OpCodes.Stfld, eventFieldBuilder);
				addEventIL.Emit(OpCodes.Ret);

				type.DefineMethodOverride(addEventBuilder, eventInfo.GetAddMethod());
				eventBuilder.SetAddOnMethod(addEventBuilder);

				#endregion

				#region remove event

				var removeEventBuilder = type.DefineMethod(
					"remove_" + eventInfo.Name, 
					eventInfo.GetRemoveMethod().Attributes & ~MethodAttributes.Abstract, 
					null, 
					new[]
					{
						eventHandleType
					});
				var removeEventIL = removeEventBuilder.GetILGenerator();
				removeEventIL.Emit(OpCodes.Ldarg_0);
				removeEventIL.Emit(OpCodes.Ldarg_0);
				removeEventIL.Emit(OpCodes.Ldfld, eventFieldBuilder);
				removeEventIL.Emit(OpCodes.Ldarg_1);
				removeEventIL.Emit(
					OpCodes.Call, 
					typeof(Delegate).GetMethod(
						"Remove", 
						new[]
						{
							typeof(Delegate), 
							typeof(Delegate)
						}));

				removeEventIL.Emit(OpCodes.Castclass, eventHandleType);
				removeEventIL.Emit(OpCodes.Stfld, eventFieldBuilder);
				removeEventIL.Emit(OpCodes.Ret);
				type.DefineMethodOverride(removeEventBuilder, eventInfo.GetRemoveMethod());
				eventBuilder.SetRemoveOnMethod(removeEventBuilder);

				#endregion
			}

			#endregion

			return type.CreateType();
		}

		private static void TestEmitYield(ILGenerator cil)
		{
			var yield = typeof(TypeHelper).GetMethod("Yield", BindingFlags.Public | BindingFlags.Static);
			cil.Emit(OpCodes.Call, yield);
		}

		public event Action<string, string> ErrorMethodEvent;
	}
}
