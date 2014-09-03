using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{
    
	using System.Reflection;
	using System.Reflection.Emit;

    public interface IAgent : Regulus.Utility.IUpdatable
    {
        Regulus.Remoting.Ghost.IProviderNotice<T> QueryProvider<T>();

        Value<bool> Connect(string account, int password);

        long Ping { get; }

        event Action DisconnectEvent;
        void Disconnect();
    }

	public class AgentCore
	{
		IGhostRequest _Requester ;
        
		public AgentCore(IGhostRequest req)
		{            
			_Requester = req;				
		}

		public void Initial()
		{
			_StartPing();
		}
		public void Finial()
		{
			_EndPing();
		}
		Regulus.Utility.TimeCounter _PingTimeCounter = new Regulus.Utility.TimeCounter();
		public long Ping { get; private set; }

		public  void OnResponse(byte id, Dictionary<byte, byte[]> args)
		{
			_OnResponse(id , args);
		}
		protected void _OnResponse(byte id, Dictionary<byte, byte[]> args)
		{
			if (id == (int)ServerToClientOpCode.Ping)
			{
				Ping = _PingTimeCounter.Ticks;
				_StartPing();
			}
			else if (id == (int)ServerToClientOpCode.UpdateProperty)
			{
				if (args.Count == 3)
				{

					var entity_id = new Guid(args[0] as byte[]);
					var eventName = Regulus.PhotonExtension.TypeHelper.Deserialize<string>(args[1] as byte[]) ;
					var value = args[2] ;

					System.Diagnostics.Debug.WriteLine("UpdateProperty id:" + entity_id + " name:" + eventName + " value:" + value);
					_UpdateProperty(entity_id, eventName, value);
				}
			}
			else if (id == (int)ServerToClientOpCode.InvokeEvent)
			{
				if (args.Count >= 2)
				{
					var entity_id = new Guid(args[0] as byte[]);
					var eventName = Regulus.PhotonExtension.TypeHelper.Deserialize<string>(args[1] as byte[]) ;
					var eventParams = (from p in args
									   where p.Key >= 2
									   select p.Value as object).ToArray();

					_InvokeEvent(entity_id, eventName, eventParams);
				}
			}
			else if (id == (int)ServerToClientOpCode.ReturnValue)
			{
				if (args.Count == 2)
				{
					var returnTarget = new Guid(args[0] as byte[]);
					var returnValue = args[1] ;

					_SetReturnValue(returnTarget, returnValue);
				}
			}
			else if (id == (int)ServerToClientOpCode.LoadSoulCompile)
			{
				if (args.Count == 2)
				{
					var typeName = Regulus.PhotonExtension.TypeHelper.Deserialize<string>(args[0] as byte[]) ;
					var entity_id = new Guid(args[1] as byte[]);
					System.Diagnostics.Debug.WriteLine("load soul compile: " + typeName + " id: " + entity_id.ToString());
					_LoadSoulCompile(typeName, entity_id);
				}
			}
			else if (id == (int)ServerToClientOpCode.LoadSoul)
			{
				if (args.Count == 2)
				{
					var typeName = Regulus.PhotonExtension.TypeHelper.Deserialize<string>(args[0] as byte[]) ;
					var entity_id = new Guid(args[1] as byte[]);
					System.Diagnostics.Debug.WriteLine("load soul : " + typeName + " id: " + entity_id.ToString());
					_LoadSoul(typeName, entity_id);
				}
			}
			else if (id == (int)ServerToClientOpCode.UnloadSoul)
			{
				if (args.Count == 2)
				{
					var typeName = Regulus.PhotonExtension.TypeHelper.Deserialize<string>(args[0] as byte[]) ;
					var entity_id = new Guid(args[1] as byte[]);
					System.Diagnostics.Debug.WriteLine("unload soul : " + typeName + " id: " + entity_id.ToString());
					_UnloadSoul(typeName, entity_id);
				}
			}
		}

		Regulus.Remoting.Ghost.ReturnValueQueue _ReturnValueQueue = new Regulus.Remoting.Ghost.ReturnValueQueue();
		private void _SetReturnValue(Guid returnTarget, byte[] returnValue)
		{
			IValue value = _ReturnValueQueue.PopReturnValue(returnTarget);
			if (value != null)
			{
				value.SetValue(returnValue);
			}
		}
		private void _LoadSoulCompile(string type_name, Guid entity_id)
		{
			Regulus.Remoting.Ghost.IProvider provider = _QueryProvider(type_name);
			if (provider != null)
				provider.Ready(entity_id);
		}
		private void _LoadSoul(string type_name, Guid id)
		{
			Regulus.Remoting.Ghost.IProvider provider = _QueryProvider(type_name);
			if (provider != null && _Requester != null)
				provider.Add(_BuildGhost(_GetType(type_name), _Requester, id));
		}

		private void _UnloadSoul(string type_name, Guid id)
		{
			var provider = _QueryProvider(type_name);
			if (provider != null)
				provider.Remove(id);
		}

		Dictionary<string, Regulus.Remoting.Ghost.IProvider> _Providers = new Dictionary<string, Regulus.Remoting.Ghost.IProvider>();
		private Regulus.Remoting.Ghost.IProvider _QueryProvider(string type_name)
		{
			Regulus.Remoting.Ghost.IProvider provider = null;
			if (_Providers.TryGetValue(type_name, out provider) == false)
			{
				var type = _GetType(type_name);
				if (type != null)
				{
					provider = _BuildProvider(type);
					_Providers.Add(type_name, provider);
				}
			}
			return provider;
		}

		private Regulus.Remoting.Ghost.IProvider _BuildProvider(Type type)
		{
			Type providerTemplateType = typeof(Regulus.Remoting.Ghost.TProvider<>);
			Type providerType = providerTemplateType.MakeGenericType(type);
			return Activator.CreateInstance(providerType) as Regulus.Remoting.Ghost.IProvider;
		}


		public Regulus.Remoting.Ghost.IProviderNotice<T> QueryProvider<T>()
		{
			return _QueryProvider(typeof(T).FullName) as Regulus.Remoting.Ghost.IProviderNotice<T>;
		}
		private void _UpdateProperty(Guid entity_id, string name, byte[] value)
		{
			Regulus.Remoting.Ghost.IGhost ghost = _FindGhost(entity_id);
			if (ghost != null)
			{
				ghost.OnProperty(name, value);
			}
		}

		private void _InvokeEvent(Guid ghost_id, string eventName, object[] eventParams)
		{
			Regulus.Remoting.Ghost.IGhost ghost = _FindGhost(ghost_id);
			if (ghost != null)
			{
				ghost.OnEvent(eventName, eventParams);
			}
		}

		private Regulus.Remoting.Ghost.IGhost _FindGhost(Guid ghost_id)
		{
			return (from provider in _Providers
					let r = (from g in provider.Value.Ghosts where ghost_id == g.GetID() select g).FirstOrDefault()
					where r != null
					select r).FirstOrDefault();
		}

        object _Sync = new object();
        enum PingStatus { Wait , Send };
        PingStatus _PingStatus;
		System.Timers.Timer _PingTimer;
		protected void _StartPing()
		{
			_EndPing();
            lock (_Sync)
            {
                _PingTimer = new System.Timers.Timer(1000);
                _PingTimer.Enabled = true;
                _PingTimer.AutoReset = true;
                _PingTimer.Elapsed += new System.Timers.ElapsedEventHandler(_PingTimerElapsed);
                _PingTimer.Start();
                _PingStatus = PingStatus.Wait;
            }
			
		}
		void _PingTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
            lock (_Sync)
            {
                if (_PingTimer != null)
                {
                    _PingStatus = PingStatus.Send;
                    _PingTimeCounter = new Regulus.Utility.TimeCounter();
                    _Requester.Request((int)ClientToServerOpCode.Ping, new Dictionary<byte, byte[]>());
                    
                }            
            }
            _EndPing();
		}

		protected void _EndPing()
		{
            lock (_Sync)
            {
                if (_PingTimer != null)
                {
                    _PingTimer.Stop();
                    _PingTimer = null;
                }
            }
		}
        
		private Regulus.Remoting.Ghost.IGhost _BuildGhost(Type ghostBaseType, Regulus.Remoting.IGhostRequest peer, Guid id)
		{
			Type ghostType = _QueryGhostType(ghostBaseType);
			object o = Activator.CreateInstance(ghostType, new Object[] { peer, id, _ReturnValueQueue }  );
            
			return (Regulus.Remoting.Ghost.IGhost)o;
		}

		static System.Collections.Generic.Dictionary<Type, Type> _GhostTypes = new Dictionary<Type, Type>();
		private Type _QueryGhostType(Type ghostBaseType)
		{
			Type ghostType = null;
			if (_GhostTypes.TryGetValue(ghostBaseType, out ghostType))
			{
				return ghostType;
			}

			ghostType = _BuildGhostType(ghostBaseType );
			_GhostTypes.Add(ghostBaseType, ghostType);
			return ghostType;
		}
        static Dictionary<string, Type> _Types = new Dictionary<string, Type>();
		static Type _GetType(string type_name)
		{
            lock (_Types)
            {
                Type result ;
                if (_Types.TryGetValue(type_name, out result))
                {
                    return result;
                }
                else
                {
                    result = _Find(type_name);
                    _Types.Add(type_name , result);
                    return result;
                }
            }
			
		}

        private static Type _Find(string type_name)
        {
            var type = Type.GetType(type_name);
            if (type == null)
            {
                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = a.GetType(type_name);
                    if (type != null)
                        return type;
                }

                throw new System.Exception("找不到gpi " + type_name);
            }
            return type;
        }


        //被_BuildGhostType參考
		public static void UpdateProperty(string property, string type_name, object instance, object value)
		{
			Type type = _GetType(type_name);
			if (type != null)
			{
				var field = type.GetField("_" + property, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				if (field != null)
				{
                    field.SetValue(instance, Regulus.PhotonExtension.TypeHelper.DeserializeObject(field.FieldType, value as byte[]) );
				}
			}
		}

        //被_BuildGhostType參考
		public static void CallEvent(string method, string type_name, object obj, object[] args)
		{
			Type type = _GetType(type_name);

			if (type != null)
			{                
				var eventInfos = type.GetField("_" + method, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				object fieldValue = eventInfos.GetValue(obj);
				if (fieldValue is Delegate)
				{                                     
					Delegate fieldValueDelegate = (fieldValue as Delegate);
                    Type[] parTypes = (from p in fieldValueDelegate.Method.GetParameters()
                                      select p.ParameterType).ToArray();
                    int i = 0;
                    object[] pars = (from a in args select Regulus.PhotonExtension.TypeHelper.DeserializeObject(parTypes[i++], a as byte[])).ToArray();
                    fieldValueDelegate.DynamicInvoke(pars);
				}
			}
		}
		static private Type _BuildGhostType(Type ghostBaseType)
		{
			//反射機制
			Type baseType = ghostBaseType;
			//產生class的組態
			AssemblyName asmName = new AssemblyName("RegulusRemotingGhost." + baseType.ToString() + "Assembly");
			//從目前的domain裡即時產生一個組態                                    
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            
            
            
			//產生一個模組
			ModuleBuilder module = assembly.DefineDynamicModule("RegulusRemotingGhost." + baseType.ToString() + "Module");
			//產生一個class or struct
			//這裡是用class
            var typeName = "C" + baseType.ToString();
            TypeBuilder type = module.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Sealed, typeof(Object), new Type[] { baseType, typeof(Regulus.Remoting.Ghost.IGhost) });
            
			#region build constructor
			// 產生建構子，有一個參數 tpeer
			ConstructorBuilder c = type.DefineConstructor(
										MethodAttributes.Public,
                                        CallingConventions.Standard,
										new Type[] { typeof(Regulus.Remoting.IGhostRequest), typeof(Guid), typeof(Regulus.Remoting.Ghost.ReturnValueQueue) });
			// 產生field，一個欄位
			FieldBuilder peerField = type.DefineField("_Peer", typeof(Regulus.Remoting.IGhostRequest), FieldAttributes.Private);
			FieldBuilder idField = type.DefineField("_ID", typeof(Guid), FieldAttributes.Private);
			FieldBuilder rvqField = type.DefineField("_ReturnValueQueue", typeof(Regulus.Remoting.Ghost.ReturnValueQueue), FieldAttributes.Private);

			// 取得中介語言的介面產生器
			ILGenerator cil = c.GetILGenerator();
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

            var objectType = typeof(Object);
            var objectTypeConstructor = objectType.GetConstructor(new Type[0]);

            cil.Emit(OpCodes.Ldarg_0); // this 指標
            cil.Emit(OpCodes.Call, objectTypeConstructor );

			cil.Emit(OpCodes.Ret); // return 出去
			#endregion

			#region build IGhostEventListener method

			var methodGetIDInfo = typeof(Regulus.Remoting.Ghost.IGhost).GetMethod("GetID");
			if (methodGetIDInfo != null)
			{
				var argTypes = (from parameter in methodGetIDInfo.GetParameters() orderby parameter.Position select parameter.ParameterType).ToArray();
				var methodBuilder = type.DefineMethod(methodGetIDInfo.Name, methodGetIDInfo.Attributes & ~MethodAttributes.Abstract, typeof(Guid), argTypes);
				var methidIL = methodBuilder.GetILGenerator();
				methidIL.Emit(OpCodes.Nop);
				methidIL.Emit(OpCodes.Ldarg_0); // this
				methidIL.Emit(OpCodes.Ldfld, idField); // id
				methidIL.Emit(OpCodes.Ret);

				type.DefineMethodOverride(methodBuilder, methodGetIDInfo);
			}

			var propertyInfos = baseType.GetProperties();
			foreach (var propertyInfo in propertyInfos)
			{
				var propertyType = propertyInfo.PropertyType;
				var field = type.DefineField("_" + propertyInfo.Name, propertyType, FieldAttributes.Private);
				var property = type.DefineProperty(propertyInfo.Name, PropertyAttributes.HasDefault, propertyType, null);

				if (propertyInfo.CanRead)
				{
					var baseMethod = propertyInfo.GetGetMethod();

					MethodBuilder method = type.DefineMethod("get_" + propertyInfo.Name, baseMethod.Attributes & ~MethodAttributes.Abstract, propertyType, Type.EmptyTypes);
					ILGenerator methodIL = method.GetILGenerator();
					methodIL.Emit(OpCodes.Ldarg_0);
					methodIL.Emit(OpCodes.Ldfld, field);
					methodIL.Emit(OpCodes.Ret);
					property.SetGetMethod(method);
					type.DefineMethodOverride(method, baseMethod);
				}

				if (propertyInfo.CanWrite)
				{
					var baseMethod = propertyInfo.GetGetMethod();
					MethodBuilder method = type.DefineMethod("set_" + propertyInfo.Name, baseMethod.Attributes & ~MethodAttributes.Abstract, null, new Type[] { propertyType });
					ILGenerator methodIL = method.GetILGenerator();
					methodIL.Emit(OpCodes.Ldarg_0);
					methodIL.Emit(OpCodes.Ldarg_1);
					methodIL.Emit(OpCodes.Stfld, field);
					methodIL.Emit(OpCodes.Ret);
					property.SetSetMethod(method);
					type.DefineMethodOverride(method, baseMethod);
				}


			}

			var methodOnProperty = typeof(Regulus.Remoting.Ghost.IGhost).GetMethod("OnProperty");
			if (methodOnProperty != null)
			{
				var argTypes = (from parameter in methodOnProperty.GetParameters() orderby parameter.Position select parameter.ParameterType).ToArray();
				MethodBuilder method = type.DefineMethod(methodOnProperty.Name, methodOnProperty.Attributes & ~MethodAttributes.Abstract, methodOnProperty.ReturnType, argTypes);
				var methodIL = method.GetILGenerator();

				methodIL.Emit(OpCodes.Ldarg_1);
				methodIL.Emit(OpCodes.Ldstr, typeName);
				methodIL.Emit(OpCodes.Ldarg_0);
				methodIL.Emit(OpCodes.Ldarg_2);                    
				methodIL.Emit(OpCodes.Call, typeof(AgentCore).GetMethod("UpdateProperty", BindingFlags.Public | BindingFlags.Static));
				methodIL.Emit(OpCodes.Nop);
				methodIL.Emit(OpCodes.Ret);

				type.DefineMethodOverride(method, methodOnProperty);
			}

			var methodOnEventInfo = typeof(Regulus.Remoting.Ghost.IGhost).GetMethod("OnEvent");
			if (methodOnEventInfo != null)
			{
				var argTypes = (from parameter in methodOnEventInfo.GetParameters() orderby parameter.Position select parameter.ParameterType).ToArray();
				var methodBuilder = type.DefineMethod(methodOnEventInfo.Name, methodOnEventInfo.Attributes & ~MethodAttributes.Abstract, null, argTypes);
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

			#region build method

			MethodInfo[] methods = baseType.GetMethods();

			foreach (var m in methods)
			{
				if (m.IsSpecialName == true)
					continue;

				//取出介面的fun，去除Abstract屬性
				MethodAttributes attribute = m.Attributes & ~(MethodAttributes.Abstract);
				//取出function的參數
				ParameterInfo[] pars = m.GetParameters();
				//取出參數的型別，用types array 裝
				Type[] types = new Type[pars.Length];
				int i = 0;
				foreach (var p in pars)
				{
					types[i++] = p.ParameterType;
				}
				//產生一個function
				MethodBuilder method = type.DefineMethod(m.Name, attribute, m.ReturnType, types);
				// 取得中介語言的介面產生器
				ILGenerator il = method.GetILGenerator();
                
				var byteArrayType = typeof(byte[]);
				LocalBuilder varGuidByteArray = il.DeclareLocal(byteArrayType);
                LocalBuilder varMethodNameByteArray = il.DeclareLocal(byteArrayType);

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, idField);
				var guidToByteArrayMethod = typeof(Regulus.PhotonExtension.TypeHelper).GetMethod("GuidToByteArray", BindingFlags.Public | BindingFlags.Static);
				il.Emit(OpCodes.Call, guidToByteArrayMethod);
				il.Emit(OpCodes.Stloc, varGuidByteArray);

                il.Emit(OpCodes.Ldstr, m.Name);
                var stringToByteArrayMethod = typeof(Regulus.PhotonExtension.TypeHelper).GetMethod("StringToByteArray", BindingFlags.Public | BindingFlags.Static);
                il.Emit(OpCodes.Call, stringToByteArrayMethod);
                il.Emit(OpCodes.Stloc, varMethodNameByteArray);


				//取出type物件
				var dictionaryType = typeof(Dictionary<byte, byte[]>);
				//宣告函式的local變數
				LocalBuilder varDict = il.DeclareLocal(dictionaryType);
				//new出指定物的建構子
				il.Emit(OpCodes.Newobj, dictionaryType.GetConstructor(Type.EmptyTypes));
				//設定local變數
				il.Emit(OpCodes.Stloc, varDict);

				// add id
				il.Emit(OpCodes.Ldloc, varDict);
				il.Emit(OpCodes.Ldc_I4, 0);
				il.Emit(OpCodes.Ldloc, varGuidByteArray);
				il.Emit(OpCodes.Call, varDict.LocalType.GetMethod("Add"));

				// add method name
				il.Emit(OpCodes.Ldloc, varDict);
				il.Emit(OpCodes.Ldc_I4, 1);
                il.Emit(OpCodes.Ldloc, varMethodNameByteArray);
				il.Emit(OpCodes.Call, varDict.LocalType.GetMethod("Add"));

				// push return info
				var valueOriType = typeof(Value<>);

                
				LocalBuilder varValueObject = null;


				if (valueOriType.Name == m.ReturnType.Name && valueOriType.Namespace == m.ReturnType.Namespace)
				{
					var argTypes = m.ReturnType.GetGenericArguments();
					var valueType = valueOriType.MakeGenericType(new Type[] { argTypes[0] });
                    
					il.Emit(OpCodes.Newobj, valueType.GetConstructor(Type.EmptyTypes));
					LocalBuilder varValue = il.DeclareLocal(valueType);
                    varValueObject = il.DeclareLocal(valueType);
					il.Emit(OpCodes.Stloc, varValue);

					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, rvqField);
					il.Emit(OpCodes.Ldloc, varValue);
					il.Emit(OpCodes.Call, rvqField.FieldType.GetMethod("PushReturnValue"));
					LocalBuilder varRVQId = il.DeclareLocal(typeof(Guid));
					il.Emit(OpCodes.Stloc, varRVQId);

					il.Emit(OpCodes.Ldloc, varRVQId);
					il.Emit(OpCodes.Call, typeof(Regulus.PhotonExtension.TypeHelper).GetMethod("GuidToByteArray", BindingFlags.Public | BindingFlags.Static));
					LocalBuilder varRVQIdByteArray = il.DeclareLocal(typeof(byte[]));
					il.Emit(OpCodes.Stloc, varRVQIdByteArray);

					il.Emit(OpCodes.Ldloc, varDict);
					il.Emit(OpCodes.Ldc_I4, 2);
					il.Emit(OpCodes.Ldloc, varRVQIdByteArray);
					il.Emit(OpCodes.Call, varDict.LocalType.GetMethod("Add"));

					il.Emit(OpCodes.Ldloc, varValue);
					il.Emit(OpCodes.Stloc, varValueObject);

				}
                
				for (int paramIndex = 0; paramIndex < pars.Length; paramIndex++)
				{
					//建立local變數，型別byte
					LocalBuilder varBuffer = il.DeclareLocal(typeof(byte[]));
					//將0  有符號的整數存到stacK
					il.Emit(OpCodes.Ldc_I4_S, 0);
					//new出byte的array
					il.Emit(OpCodes.Newarr, typeof(byte));
					//將array的值設定到varBuffer
					il.Emit(OpCodes.Stloc, varBuffer);

					//讀取參數的值 從0開始
					il.Emit(OpCodes.Ldarg, 1 + paramIndex);
					//將參數真正的型別轉出來
					//il.Emit(OpCodes.Box, types[paramIndex]);

                    //使用TypeHelper類別裡的Serializer函式 屬性為Public Static..
                    var serializer = typeof(Regulus.PhotonExtension.TypeHelper).GetMethod("Serializer", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(new Type[] { types[paramIndex] });
					//指定呼叫函式的多載，因為沒有多載，所以填null
					il.EmitCall(OpCodes.Call, serializer, null);
					//byte array 存到 varBuffer
					il.Emit(OpCodes.Stloc, varBuffer);

                    
					il.Emit(OpCodes.Ldloc, varDict);
					il.Emit(OpCodes.Ldc_I4, 3 + paramIndex);
					il.Emit(OpCodes.Ldloc, varBuffer);					

					il.Emit(OpCodes.Call, varDict.LocalType.GetMethod("Add"));                    
				}

                TestEmitYield(il);
				//填函式要的資料
				il.Emit(OpCodes.Ldarg_0); // this
				il.Emit(OpCodes.Ldfld, peerField); // peer
				il.Emit(OpCodes.Ldc_I4, (int)ClientToServerOpCode.CallMethod); // opcode 
				il.Emit(OpCodes.Ldloc, varDict);

				//指定呼叫函式的多載
				il.Emit(OpCodes.Callvirt, peerField.FieldType.GetMethod("Request", new Type[] { typeof(byte), dictionaryType }));

                TestEmitYield(il);
                
				if (valueOriType.Name == m.ReturnType.Name && valueOriType.Namespace == m.ReturnType.Namespace)
				{
					il.Emit(OpCodes.Ldloc, varValueObject);
				}
                TestEmitYield(il);
				//return;
				il.Emit(OpCodes.Ret);
				//指定覆寫的fun
				type.DefineMethodOverride(method, m);
			}
			#endregion
			#region build event
			var eventInfos = baseType.GetEvents();
			foreach (var eventInfo in eventInfos)
			{
				string eventName = "_" + eventInfo.Name;
				Type eventHandleType = eventInfo.EventHandlerType;
				var eventFieldBuilder = type.DefineField(eventName, eventHandleType, FieldAttributes.FamORAssem);
				var eventBuilder = type.DefineEvent(eventInfo.Name, EventAttributes.None, eventHandleType);

				#region add event
				var addEventBuilder = type.DefineMethod("add_" + eventInfo.Name, eventInfo.GetAddMethod().Attributes & ~(MethodAttributes.Abstract), null, new Type[] { eventHandleType });
				var addEventIL = addEventBuilder.GetILGenerator();
				addEventIL.Emit(OpCodes.Ldarg_0);
				addEventIL.Emit(OpCodes.Ldarg_0);
				addEventIL.Emit(OpCodes.Ldfld, eventFieldBuilder);
				addEventIL.Emit(OpCodes.Ldarg_1);
				addEventIL.Emit(OpCodes.Call, typeof(Delegate).GetMethod("Combine", new Type[] { typeof(Delegate), typeof(Delegate) }));

				addEventIL.Emit(OpCodes.Castclass, eventHandleType);
				addEventIL.Emit(OpCodes.Stfld, eventFieldBuilder);
				addEventIL.Emit(OpCodes.Ret);

				type.DefineMethodOverride(addEventBuilder, eventInfo.GetAddMethod());
				eventBuilder.SetAddOnMethod(addEventBuilder);
				#endregion

				#region remove event
				var removeEventBuilder = type.DefineMethod("remove_" + eventInfo.Name, eventInfo.GetRemoveMethod().Attributes & ~(MethodAttributes.Abstract), null, new Type[] { eventHandleType });
				var removeEventIL = removeEventBuilder.GetILGenerator();
				removeEventIL.Emit(OpCodes.Ldarg_0);
				removeEventIL.Emit(OpCodes.Ldarg_0);
				removeEventIL.Emit(OpCodes.Ldfld, eventFieldBuilder);
				removeEventIL.Emit(OpCodes.Ldarg_1);
				removeEventIL.Emit(OpCodes.Call, typeof(Delegate).GetMethod("Remove", new Type[] { typeof(Delegate), typeof(Delegate) }));

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
			var yield = typeof(Regulus.PhotonExtension.TypeHelper).GetMethod("Yield", BindingFlags.Public | BindingFlags.Static);
			cil.Emit(OpCodes.Call, yield);
		}
	}
}
