using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System;
using System.Linq;

namespace Regulus.Remote.Ghost
{
    public class DynamicProvider 
    {
        Type Find(Type ghost_base_type)
        {
            // 反射機制
            var baseType = ghost_base_type;

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
            if (methodGetIDInfo != null)
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
            if (methodIsReturnTypeInfo != null)
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
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyType = propertyInfo.PropertyType;
                var field = type.DefineField("_" + propertyInfo.Name, propertyType, FieldAttributes.Private);
                var property = type.DefineProperty(propertyInfo.Name, PropertyAttributes.HasDefault, propertyType, null);

                if (propertyInfo.CanRead)
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

                if (propertyInfo.CanWrite)
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
            if (methodOnProperty != null)
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
            if (methodOnEventInfo != null)
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

            foreach (var m in methods)
            {
                if (m.IsSpecialName)
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
                foreach (var p in pars)
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
               /* var guidToByteArrayMethod = typeof(TypeHelper).GetMethod(
                    "GuidToByteArray",
                    BindingFlags.Public | BindingFlags.Static);
                il.Emit(OpCodes.Call, guidToByteArrayMethod);
                il.Emit(OpCodes.Stloc, varGuidByteArray);

                il.Emit(OpCodes.Ldstr, m.Name);
                var stringToByteArrayMethod = typeof(TypeHelper).GetMethod(
                    "StringToByteArray",
                    BindingFlags.Public | BindingFlags.Static);
                il.Emit(OpCodes.Call, stringToByteArrayMethod);*/
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

                if (valueOriType.Name == m.ReturnType.Name && valueOriType.Namespace == m.ReturnType.Namespace)
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
                    //il.Emit(OpCodes.Call, typeof(TypeHelper).GetMethod("GuidToByteArray", BindingFlags.Public | BindingFlags.Static));
                    var varRVQIdByteArray = il.DeclareLocal(typeof(byte[]));
                    il.Emit(OpCodes.Stloc, varRVQIdByteArray);

                    il.Emit(OpCodes.Ldloc, varDict);
                    il.Emit(OpCodes.Ldc_I4, 2);
                    il.Emit(OpCodes.Ldloc, varRVQIdByteArray);
                    il.Emit(OpCodes.Call, varDict.LocalType.GetMethod("Add"));

                    il.Emit(OpCodes.Ldloc, varValue);
                    il.Emit(OpCodes.Stloc, varValueObject);
                }

                for (var paramIndex = 0; paramIndex < pars.Length; paramIndex++)
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
                    /*var serializer =
                        typeof(TypeHelper).GetMethod("Serialize", BindingFlags.Public | BindingFlags.Static)
                                          .MakeGenericMethod(types[paramIndex]);

                    // 指定呼叫函式的多載，因為沒有多載，所以填null
                    il.EmitCall(OpCodes.Call, serializer, null);*/

                    // byte array 存到 varBuffer
                    il.Emit(OpCodes.Stloc, varBuffer);

                    il.Emit(OpCodes.Ldloc, varDict);
                    il.Emit(OpCodes.Ldc_I4, 3 + paramIndex);
                    il.Emit(OpCodes.Ldloc, varBuffer);

                    il.Emit(OpCodes.Call, varDict.LocalType.GetMethod("Add"));
                }

                

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

                

                if (valueOriType.Name == m.ReturnType.Name && valueOriType.Namespace == m.ReturnType.Namespace)
                {
                    il.Emit(OpCodes.Ldloc, varValueObject);
                }

                
                // return;
                il.Emit(OpCodes.Ret);

                // 指定覆寫的fun
                type.DefineMethodOverride(method, m);
            }

            #endregion

            #region build event

            var eventInfos = baseType.GetEvents();
            foreach (var eventInfo in eventInfos)
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
    }
}
