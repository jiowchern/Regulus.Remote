using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Reflection.Emit;


namespace Samebest.Remoting.Ghost
{
	public class Agent : ExitGames.Client.Photon.IPhotonPeerListener
	{
		Samebest.Remoting.PhotonExtension.ClientPeer _Peer;	
		LinkState							_LinkState;
		Config								_Config;

		public Agent(Config config)
		{			
			_Config = config;
			
		}

		public void Launch(LinkState link_state)
		{			
			_LinkState = link_state;
			_Peer = new Samebest.Remoting.PhotonExtension.ClientPeer(this, ExitGames.Client.Photon.ConnectionProtocol.Udp);			
			_Peer.Connect(_Config.Address , _Config.Name);
			
		}

		public bool Update()
		{
            if (_Peer != null )
			{
				_Peer.Service();
				return true;
			}
			return false;				
		}

		public void Shutdown()
		{
            if (_Peer != null)
            {
                _Peer.Disconnect();
                _Peer = null;
            }
			
		}

		void ExitGames.Client.Photon.IPhotonPeerListener.DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
		{
		//	throw new NotImplementedException();
		}

		void ExitGames.Client.Photon.IPhotonPeerListener.OnEvent(ExitGames.Client.Photon.EventData eventData)
		{
		//	throw new NotImplementedException();
		}

		void ExitGames.Client.Photon.IPhotonPeerListener.OnOperationResponse(ExitGames.Client.Photon.OperationResponse operationResponse)
		{			
			if (operationResponse.OperationCode == (int)ServerToClientPhotonOpCode.InvokeEvent)
			{
				if (operationResponse.Parameters.Count >= 2)
				{
					var entity_id = new Guid(operationResponse.Parameters[0] as byte[]);
                    var eventName = Samebest.PhotonExtension.TypeHelper.Deserialize(operationResponse.Parameters[1] as byte[]) as string;
					var eventParams = (from p in operationResponse.Parameters
										where p.Key >= 2
										select Samebest.PhotonExtension.TypeHelper.Deserialize(p.Value as byte[])).ToArray();

					_InvokeEvent(entity_id, eventName, eventParams);
				}
			}
			else if (operationResponse.OperationCode == (int)ServerToClientPhotonOpCode.ReturnValue)
			{
				if (operationResponse.Parameters.Count == 2)
				{
					var returnTarget  = new Guid(operationResponse.Parameters[0] as byte[]);
					var returnValue = Samebest.PhotonExtension.TypeHelper.Deserialize(operationResponse.Parameters[1] as byte[]);

					_SetReturnValue(returnTarget, returnValue);
				}
			}
			else if (operationResponse.OperationCode == (int)ServerToClientPhotonOpCode.LoadSoul)
			{
				if (operationResponse.Parameters.Count == 2)
				{
                    var typeName = Samebest.PhotonExtension.TypeHelper.Deserialize(operationResponse.Parameters[0] as byte[]) as string;
					var entity_id = new Guid(operationResponse.Parameters[1] as byte[]);
					_LoadSoul(typeName, entity_id);
				}
			}
			else if (operationResponse.OperationCode == (int)ServerToClientPhotonOpCode.UnloadSoul)
			{
				if (operationResponse.Parameters.Count == 2)
				{
                    var typeName = Samebest.PhotonExtension.TypeHelper.Deserialize(operationResponse.Parameters[0] as byte[])as string;
					var entity_id = new Guid(operationResponse.Parameters[1] as byte[]);
					_UnloadSoul(typeName, entity_id);
				}
			}
			
		}


		Samebest.Remoting.Ghost.ReturnValueQueue _ReturnValueQueue = new Samebest.Remoting.Ghost.ReturnValueQueue();
		private void _SetReturnValue(Guid returnTarget, object returnValue)
		{
			IValue value = _ReturnValueQueue.PopReturnValue(returnTarget);
			if (value != null)
			{
				value.SetValue(returnValue);
			}
		}
		
		private void _LoadSoul(string type_name, Guid id)
		{
			IProvider provider = _QueryProvider(type_name);
			if (provider != null)
				provider.Add(_BuildGhost(_GetType(type_name), _Peer, id));
		}

		private void _UnloadSoul(string type_name, Guid id)
		{
			var provider = _QueryProvider(type_name);
			if (provider != null)
				provider.Remove(id);
		}

		Dictionary<string , IProvider>	_Providers = new Dictionary<string,IProvider>();
		private IProvider _QueryProvider(string type_name)
		{
			IProvider provider = null;
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

		private IProvider _BuildProvider(Type type)
		{
			Type providerTemplateType = typeof(TProvider<>);
			Type providerType = providerTemplateType.MakeGenericType(type);
			return Activator.CreateInstance(providerType) as IProvider;
		}
		

		public IProviderNotice<T> QueryProvider<T>()
		{
			return _QueryProvider(typeof(T).FullName) as IProviderNotice<T>;	
		}
		private void _InvokeEvent(Guid ghost_id, string eventName, object[] eventParams)
		{
			IGhost ghost = _FindGhost(ghost_id);
			if (ghost != null)
			{
				ghost.OnEvent(eventName, eventParams);
			}			
		}

		private IGhost _FindGhost(Guid ghost_id)
		{
			return	(from provider in _Providers 
					select (from g in provider.Value.Ghosts where ghost_id == g.GetID() select g).FirstOrDefault()).FirstOrDefault();	
		}
		
		

		void ExitGames.Client.Photon.IPhotonPeerListener.OnStatusChanged(ExitGames.Client.Photon.StatusCode statusCode)
		{
			if (statusCode == ExitGames.Client.Photon.StatusCode.Connect)
			{
				if (_LinkState.LinkSuccess != null)
					_LinkState.LinkSuccess.Invoke();
				_StartPing();
			}
            else if (statusCode == ExitGames.Client.Photon.StatusCode.QueueOutgoingReliableWarning)
            {
            }
            else if (statusCode == ExitGames.Client.Photon.StatusCode.QueueIncomingReliableWarning)
            {
            }
            else if (statusCode == ExitGames.Client.Photon.StatusCode.QueueOutgoingAcksWarning)
            {
            }
            else if (statusCode == ExitGames.Client.Photon.StatusCode.QueueSentWarning)
            {
            }
            else if (statusCode == ExitGames.Client.Photon.StatusCode.SendError)
            {

            }
            else
            {
                
                _EndPing();
                if (_LinkState.LinkFail != null)
                    _LinkState.LinkFail();
                _Peer = null;
            }

            System.Diagnostics.Debug.WriteLine(statusCode.ToString());
		}

		System.Timers.Timer _PingTimer;
		private void _StartPing()
		{
			_EndPing();
			_PingTimer = new System.Timers.Timer(1000);
			_PingTimer.Enabled = true;
			_PingTimer.AutoReset = true;
			_PingTimer.Elapsed += new System.Timers.ElapsedEventHandler(_PingTimerElapsed);
			_PingTimer.Start();
		}

		void _PingTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (_Peer != null)
				_Peer.OpCustom((int)ClientToServerPhotonOpCode.Ping, new Dictionary<byte, object>(), false);
		}

		private void _EndPing()
		{
			if (_PingTimer != null)
			{
				_PingTimer.Stop();
				_PingTimer = null;
			}

		}

		

		private IGhost _BuildGhost(Type ghostBaseType, Samebest.Remoting.PhotonExtension.ClientPeer peer , Guid id) 
		{
			Type ghostType = _QueryGhostType(ghostBaseType);
			object o = Activator.CreateInstance(ghostType, new Object[] { peer, id, _ReturnValueQueue });
			return (IGhost)o;
		}

		static System.Collections.Generic.Dictionary<Type , Type> _GhostTypes = new Dictionary<Type,Type>();
		private Type _QueryGhostType(Type ghostBaseType)
		{
			Type ghostType = null;
			if (_GhostTypes.TryGetValue(ghostBaseType, out ghostType))
			{
				return ghostType;
			}

			ghostType = _BuildGhostType(ghostBaseType);
			_GhostTypes.Add(ghostBaseType, ghostType);
			return ghostType;
		}
		static Type _GetType(string type_name)
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
			}
			return type;
		}

		public static void CallEvent(string method, string type_name, object obj, object[] args)
		{
			Type type = _GetType(type_name);

			if (type != null)
			{
				var eventInfos = type.GetField("_" + method, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				object fieldValue = eventInfos.GetValue(obj);
				if (fieldValue is Delegate)
				{
					(fieldValue as Delegate).DynamicInvoke(args);
				}
			}
		}
		static private Type _BuildGhostType(Type ghostBaseType)
		{
			//反射機制
			Type baseType = ghostBaseType;
			//產生class的組態
			AssemblyName asmName = new AssemblyName("SamebestRemotingGhost." + baseType.ToString() + "Assembly");
			//從目前的domain裡即時產生一個組態
			AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
			//產生一個模組
			ModuleBuilder module = assembly.DefineDynamicModule("SamebestRemotingGhost." + baseType.ToString() + "Module");
			//產生一個class or struct
			//這裡是用class
			var typeName = "C" + baseType.ToString();
			TypeBuilder type = module.DefineType(typeName, TypeAttributes.Class, typeof(Object), new Type[] { baseType, typeof(Samebest.Remoting.Ghost.IGhost) });

			#region build constructor
			// 產生建構子，有一個參數 tpeer
			ConstructorBuilder c = type.DefineConstructor(
										MethodAttributes.Public,
										CallingConventions.Standard,
										new Type[] { typeof(Samebest.Remoting.PhotonExtension.ClientPeer), typeof(Guid), typeof(Samebest.Remoting.Ghost.ReturnValueQueue) });
			// 產生field，一個欄位
			FieldBuilder peerField = type.DefineField("_Peer", typeof(Samebest.Remoting.PhotonExtension.ClientPeer), FieldAttributes.Private);
			FieldBuilder idField = type.DefineField("_ID", typeof(Guid), FieldAttributes.Private);
			FieldBuilder rvqField = type.DefineField("_ReturnValueQueue", typeof(Samebest.Remoting.Ghost.ReturnValueQueue), FieldAttributes.Private);

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
			cil.Emit(OpCodes.Ret); // return 出去
			#endregion

			#region build IGhostEventListener method

			var methodGetIDInfo = typeof(IGhost).GetMethod("GetID");
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

			var methodOnEventInfo = typeof(IGhost).GetMethod("OnEvent");
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
				methidIL.Emit(OpCodes.Call, typeof(Agent).GetMethod("CallEvent", BindingFlags.Public | BindingFlags.Static));
				methidIL.Emit(OpCodes.Nop);
				methidIL.Emit(OpCodes.Ret);

				type.DefineMethodOverride(methodBuilder, methodOnEventInfo);
			}
			

			
			#endregion

			#region build method

			MethodInfo[] methods = baseType.GetMethods();

			foreach (var m in methods)
			{
				if(m.IsSpecialName == true)
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

				var guidByteArrayType = typeof(byte[]);
				LocalBuilder varGuidByteArray = il.DeclareLocal(guidByteArrayType);
				
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, idField);
				var guidToByteArrayMethod = typeof(Samebest.PhotonExtension.TypeHelper).GetMethod("GuidToByteArray", BindingFlags.Public | BindingFlags.Static);
				il.Emit(OpCodes.Call, guidToByteArrayMethod );
				il.Emit(OpCodes.Stloc, varGuidByteArray);				

				
				//取出type物件
				var dictionaryType = typeof(Dictionary<byte, object>);
				//宣告函式的local變數
				LocalBuilder varDict = il.DeclareLocal(dictionaryType);
				//new出指定物的建構子
				il.Emit(OpCodes.Newobj, dictionaryType.GetConstructor(Type.EmptyTypes));
				//設定local變數
				il.Emit(OpCodes.Stloc, varDict);

				// add id
				il.Emit(OpCodes.Ldloc, varDict);
				il.Emit(OpCodes.Ldc_I4, 0);
				il.Emit(OpCodes.Ldloc , varGuidByteArray);				
				il.Emit(OpCodes.Call, varDict.LocalType.GetMethod("Add"));

				// add method name
				il.Emit(OpCodes.Ldloc, varDict);
				il.Emit(OpCodes.Ldc_I4, 1);
				il.Emit(OpCodes.Ldstr, m.Name);
				il.Emit(OpCodes.Call, varDict.LocalType.GetMethod("Add"));

				// push return info
				var valueOriType = typeof(Value<>);
				LocalBuilder varValueObject = il.DeclareLocal(typeof(object));

				
				if (valueOriType.Name == m.ReturnType.Name && valueOriType.Namespace == m.ReturnType.Namespace)
				{
					var argTypes = m.ReturnType.GetGenericArguments();
					var valueType = valueOriType.MakeGenericType(new Type[] { argTypes[0] });
					il.Emit(OpCodes.Newobj, valueType.GetConstructor(Type.EmptyTypes));
					LocalBuilder varValue = il.DeclareLocal(valueType);
					il.Emit(OpCodes.Stloc, varValue);
					
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, rvqField);
					il.Emit(OpCodes.Ldloc, varValue);
					il.Emit(OpCodes.Call, rvqField.FieldType.GetMethod("PushReturnValue"));
					LocalBuilder varRVQId = il.DeclareLocal(typeof(Guid));
					il.Emit(OpCodes.Stloc, varRVQId);

					il.Emit(OpCodes.Ldloc, varRVQId);
					il.Emit(OpCodes.Call, typeof(Samebest.PhotonExtension.TypeHelper).GetMethod("GuidToByteArray", BindingFlags.Public | BindingFlags.Static));
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

					//使用TypeHelper類別裡的Serializer函式 屬性為Public Static..
					var serializer = typeof(Samebest.PhotonExtension.TypeHelper).GetMethod("Serializer", BindingFlags.Public | BindingFlags.Static);

					//讀取參數的值 從0開始
					il.Emit(OpCodes.Ldarg, 1 + paramIndex);
					//將參數真正的型別轉出來
					il.Emit(OpCodes.Box, types[paramIndex]);
					//指定呼叫函式的多載，因為沒有多載，所以填null
					il.EmitCall(OpCodes.Call, serializer, null);
					//byte array 存到 varBuffer
					il.Emit(OpCodes.Stloc, varBuffer);
					//
					il.Emit(OpCodes.Ldloc, varDict);
					il.Emit(OpCodes.Ldc_I4, 3 + paramIndex);
					il.Emit(OpCodes.Ldloc, varBuffer);
					il.Emit(OpCodes.Box, typeof(byte[]));
					
					il.Emit(OpCodes.Call, varDict.LocalType.GetMethod("Add"));
				}

				//填函式要的資料
				il.Emit(OpCodes.Ldarg_0); // this
				il.Emit(OpCodes.Ldfld, peerField); // peer
				il.Emit(OpCodes.Ldc_I4, (int)ClientToServerPhotonOpCode.CallMethod); // opcode 
				il.Emit(OpCodes.Ldloc, varDict);
				
				il.Emit(OpCodes.Ldc_I4, 1);
				//指定呼叫函式的多載
				il.Emit(OpCodes.Call, peerField.FieldType.GetMethod("OpCustom", new Type[] { typeof(byte), dictionaryType, typeof(bool) }));
				//把return的值pop掉
				il.Emit(OpCodes.Pop);

				if (valueOriType.Name == m.ReturnType.Name && valueOriType.Namespace == m.ReturnType.Namespace)
				{
					il.Emit(OpCodes.Ldloc, varValueObject);
				}
				
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
			var yield = typeof(Samebest.PhotonExtension.TypeHelper).GetMethod("Yield", BindingFlags.Public | BindingFlags.Static);
			cil.Emit(OpCodes.Call, yield);
		}

		
	}
}
