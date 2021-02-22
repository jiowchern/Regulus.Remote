using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Regulus.Remote.Protocol
{
    public class CodeBuilder
    {
        static readonly string _GhostIdName = "_GhostIdName";



        public event Action<string, string> ProviderEvent;
        public event Action<string, string> GpiEvent;
        public event Action<string, string, string> EventEvent;



        public void Build(System.Reflection.Assembly assembly)
        {
            var baseName = assembly.FullName;
            Type[] types = assembly.GetExportedTypes();
            List<string> codeGpis = new List<string>();
            List<string> codeEvents = new List<string>();


            List<string> addGhostType = new List<string>();
            List<string> addEventType = new List<string>();

            HashSet<Type> serializerTypes = new HashSet<Type>();

            List<string> memberMapMethodBuilder = new List<string>();
            List<string> memberMapEventBuilder = new List<string>();
            List<string> memberMapPropertyBuilder = new List<string>();

            List<string> memberMapInterfaceBuilder = new List<string>();
            foreach (Type type in types)
            {

                serializerTypes.Add(type);


                if (_ValidRemoteInterface(type))
                {
                    string ghostClassCode = _BuildGhostCode(type);
                    string typeName = _GetTypeName(type);
                    addGhostType.Add($"types.Add(typeof({typeName}) , typeof({_GetGhostType(type)}) );");
                    codeGpis.Add(ghostClassCode);
                    if (GpiEvent != null)
                        GpiEvent(typeName, ghostClassCode);

                    EventInfo[] eventInfos = type.GetEvents();
                    foreach (EventInfo eventInfo in eventInfos)
                    {
                        addEventType.Add($"eventClosures.Add(new {_GetEventType(type, eventInfo.Name)}() );");
                        string eventCode = _BuildEventCode(type, eventInfo);
                        codeEvents.Add(eventCode);

                        if (EventEvent != null)
                            EventEvent(typeName, eventInfo.Name, eventCode);


                        memberMapEventBuilder.Add(String.Format("typeof({0}).GetEvent(\"{1}\")", type.FullName, eventInfo.Name));
                    }

                    MethodInfo[] methodInfos = type.GetMethods();
                    foreach (MethodInfo methodInfo in methodInfos)
                    {
                        if (methodInfo.IsPublic && methodInfo.IsSpecialName == false)
                        {
                            memberMapMethodBuilder.Add(_BuildGetTypeMethodInfo(methodInfo));
                        }
                    }


                    PropertyInfo[] propertyInfos = type.GetProperties();

                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {

                        memberMapPropertyBuilder.Add(String.Format("typeof({0}).GetProperty(\"{1}\")", type.FullName, propertyInfo.Name));
                    }

                    memberMapInterfaceBuilder.Add(String.Format("new System.Tuple<System.Type, System.Func<Regulus.Remote.IProvider>>(typeof({0}),()=>new Regulus.Remote.TProvider<{0}>())", type.FullName));
                }


            }
            string addMemberMapinterfaceCode = string.Join(",", memberMapInterfaceBuilder.ToArray());
            string addMemberMapPropertyCode = string.Join(",", memberMapPropertyBuilder.ToArray());
            string addMemberMapEventCode = string.Join(",", memberMapEventBuilder.ToArray());

            string addMemberMapMethodCode = string.Join(",", memberMapMethodBuilder.ToArray());
            string addTypeCode = string.Join("\n", addGhostType.ToArray());
            string addDescriberCode = string.Join(",", _GetSerializarType(serializerTypes));
            string addEventCode = string.Join("\n", addEventType.ToArray());
            
            string providerNamespaceHead = "";
            string providerNamespaceTail = "";            


            StringBuilder builder = new StringBuilder();
            builder.Append(addTypeCode);
            builder.Append(addEventCode);
            builder.Append(addDescriberCode);

            byte[] md5 = _BuildMd5(builder);
            string verificationCode = _BuildVerificationCode(md5);
            string procotolName = _BuildProtocolName(md5);
            string providerCode =
                $@"
            using System;  
            using System.Collections.Generic;
            
            {providerNamespaceHead}
                public class {procotolName} : Regulus.Remote.IProtocol
                {{
                    readonly Regulus.Remote.InterfaceProvider _InterfaceProvider;
                    readonly Regulus.Remote.EventProvider _EventProvider;
                    readonly Regulus.Remote.MemberMap _MemberMap;
                    readonly Regulus.Serialization.ISerializer _Serializer;
                    readonly System.Reflection.Assembly _Base;
                    public {procotolName}()
                    {{
                        _Base = System.Reflection.Assembly.Load(""{baseName}"");
                        var types = new Dictionary<Type, Type>();
                        {addTypeCode}
                        _InterfaceProvider = new Regulus.Remote.InterfaceProvider(types);
                        var eventClosures = new List<Regulus.Remote.IEventProxyCreator>();
                        {addEventCode}
                        _EventProvider = new Regulus.Remote.EventProvider(eventClosures);
                        _Serializer = new Regulus.Serialization.Serializer(new Regulus.Serialization.DescriberBuilder({addDescriberCode}).Describers);
                        _MemberMap = new Regulus.Remote.MemberMap(new System.Reflection.MethodInfo[] {{{addMemberMapMethodCode}}} ,new System.Reflection.EventInfo[]{{ {addMemberMapEventCode} }}, new System.Reflection.PropertyInfo[] {{{addMemberMapPropertyCode} }}, new System.Tuple<System.Type, System.Func<Regulus.Remote.IProvider>>[] {{{addMemberMapinterfaceCode}}});
                    }}
                    System.Reflection.Assembly Regulus.Remote.IProtocol.Base => _Base;
                    byte[] Regulus.Remote.IProtocol.VerificationCode {{ get {{ return new byte[]{{{verificationCode}}};}} }}
                    Regulus.Remote.InterfaceProvider Regulus.Remote.IProtocol.GetInterfaceProvider()
                    {{
                        return _InterfaceProvider;
                    }}

                    Regulus.Remote.EventProvider Regulus.Remote.IProtocol.GetEventProvider()
                    {{
                        return _EventProvider;
                    }}

                    Regulus.Serialization.ISerializer Regulus.Remote.IProtocol.GetSerialize()
                    {{
                        return _Serializer;
                    }}

                    Regulus.Remote.MemberMap Regulus.Remote.IProtocol.GetMemberMap()
                    {{
                        return _MemberMap;
                    }}
                    
                }}
            {providerNamespaceTail}
            ";

            if (ProviderEvent != null)
                ProviderEvent(procotolName, providerCode);
        }

        private bool _ValidRemoteInterface(Type type)
        {

            return type.IsInterface;
        }

        internal string _BuildGetTypeMethodInfo(MethodInfo method_info)
        {

            string methodCode = method_info.Name;
            IEnumerable<Type> argTypes = method_info.GetParameters().Select(p => p.ParameterType);
            string argTypesCode = _GetTypes(new[] { method_info.DeclaringType }.Concat(argTypes).ToArray());
            string argInstanceCode = method_info.GetParameters().Length > 0 ? "ins," : "ins";

            string paramCode = _BuildAddParams(method_info);

            string code =
                $"new Regulus.Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<System.Action{argTypesCode}>)(({argInstanceCode}{paramCode}) => ins.{methodCode}({paramCode}))).Method";
            return code;
        }

        private byte[] _BuildMd5(StringBuilder builder)
        {
            MD5 md5 = MD5.Create();
            return md5.ComputeHash(Encoding.ASCII.GetBytes(builder.ToString()));
        }
        private string _BuildProtocolName(byte[] code)
        {
            return $"C{BitConverter.ToString(code).Replace("-", "")}";
        }
        private string _BuildVerificationCode(byte[] code)
        {
            Regulus.Utility.Log.Instance.WriteInfo("Verification Code " + Convert.ToBase64String(code));
            return string.Join(",", code.Select(val => val.ToString()).ToArray());
        }

        private string[] _GetSerializarType(HashSet<Type> serializer_types)
        {
            HashSet<Type> types = new HashSet<Type>();

            serializer_types.Add(typeof(Regulus.Remote.PackageProtocolSubmit));
            serializer_types.Add(typeof(Regulus.Remote.RequestPackage));
            serializer_types.Add(typeof(Regulus.Remote.ResponsePackage));
            serializer_types.Add(typeof(Regulus.Remote.PackageInvokeEvent));
            serializer_types.Add(typeof(Regulus.Remote.PackageErrorMethod));
            serializer_types.Add(typeof(Regulus.Remote.PackageReturnValue));
            serializer_types.Add(typeof(Regulus.Remote.PackageLoadSoulCompile));
            serializer_types.Add(typeof(Regulus.Remote.PackageLoadSoul));
            serializer_types.Add(typeof(Regulus.Remote.PackageUnloadSoul));
            serializer_types.Add(typeof(Regulus.Remote.PackageCallMethod));
            serializer_types.Add(typeof(Regulus.Remote.PackageRelease));
            serializer_types.Add(typeof(Regulus.Remote.PackageSetProperty));
            serializer_types.Add(typeof(Regulus.Remote.PackageSetPropertyDone));
            serializer_types.Add(typeof(Regulus.Remote.PackageAddEvent));
            serializer_types.Add(typeof(Regulus.Remote.PackageRemoveEvent));
            serializer_types.Add(typeof(Regulus.Remote.PackageNotifierEventHook));
            serializer_types.Add(typeof(Regulus.Remote.PackageNotifier));

            foreach (Type serializerType in serializer_types)
            {
                foreach (Type type in new TypeDisintegrator(serializerType).Types)
                {
                    types.Add(type);
                }
            }
            string[] typeCodes = (from type in types orderby type.FullName select "typeof(" + _GetTypeName(type) + ")").ToArray();

            foreach (Type type in types)
            {
                Regulus.Utility.Log.Instance.WriteInfo(type.FullName);
            }
            Regulus.Utility.Log.Instance.WriteInfo("Serializable object " + types.Count);
            return typeCodes;
        }

        private string _GetGhostType(Type type)
        {
            return $"{type.Namespace}.Ghost.C{type.Name}";
        }

        private string _GetEventType(Type type, string event_name)
        {

            return $"{type.Namespace}.Invoker.{type.Name}.{event_name}";
        }

        private string _BuildEventCode(Type type, EventInfo info)
        {
            string nameSpace = type.Namespace;
            string name = type.Name;

            Type[] argTypes = info.EventHandlerType.GetGenericArguments();
            string eventName = info.Name;
            return $@"
    using System;  
    using System.Collections.Generic;
    
    namespace {nameSpace}.Invoker.{name} 
    {{ 
        public class {eventName} : Regulus.Remote.IEventProxyCreator
        {{

            Type _Type;
            string _Name;
            
            public {eventName}()
            {{
                _Name = ""{eventName}"";
                _Type = typeof({type.FullName});                   
            
            }}
            Delegate Regulus.Remote.IEventProxyCreator.Create(long soul_id,int event_id,long handler_id, Regulus.Remote.InvokeEventCallabck invoke_Event)
            {{                
                var closure = new Regulus.Remote.GenericEventClosure{_GetTypes(argTypes)}(soul_id , event_id ,handler_id, invoke_Event);                
                return new Action{_GetTypes(argTypes)}(closure.Run);
            }}
        

            Type Regulus.Remote.IEventProxyCreator.GetType()
            {{
                return _Type;
            }}            

            string Regulus.Remote.IEventProxyCreator.GetName()
            {{
                return _Name;
            }}            
        }}
    }}
                ";




        }
        private string _BuildGhostCode(Type type)
        {
            string nameSpace = type.Namespace;

            string name = type.Name;

            IEnumerable<Type> types = type.GetInterfaces().Concat(
                new[]
                {
                    type
                });
            string implementCode = _BuildGhostCode(types);
            string codeHeader =
$@"   
    using System;  
    
    using System.Collections.Generic;
    
    namespace {nameSpace}.Ghost 
    {{ 
        public class C{name} : {_GetTypeName(type)} , Regulus.Remote.IGhost
        {{
            readonly bool _HaveReturn ;
            
            readonly long {CodeBuilder._GhostIdName};
            
            
            
            public C{name}(long id, bool have_return )
            {{                                
                _HaveReturn = have_return ;
                {CodeBuilder._GhostIdName} = id; 
                
            }}
            

            long Regulus.Remote.IGhost.GetID()
            {{
                return {CodeBuilder._GhostIdName};
            }}

            bool Regulus.Remote.IGhost.IsReturnType()
            {{
                return _HaveReturn;
            }}
            object Regulus.Remote.IGhost.GetInstance()
            {{
                return this;
            }}

            private event Regulus.Remote.CallMethodCallback _CallMethodEvent;

            event Regulus.Remote.CallMethodCallback Regulus.Remote.IGhost.CallMethodEvent
            {{
                add {{ this._CallMethodEvent += value; }}
                remove {{ this._CallMethodEvent -= value; }}
            }}

            private event Regulus.Remote.EventNotifyCallback _AddEventEvent;

            event Regulus.Remote.EventNotifyCallback Regulus.Remote.IGhost.AddEventEvent
            {{
                add {{ this._AddEventEvent += value; }}
                remove {{ this._AddEventEvent -= value; }}
            }}

            private event Regulus.Remote.EventNotifyCallback _RemoveEventEvent;

            event Regulus.Remote.EventNotifyCallback Regulus.Remote.IGhost.RemoveEventEvent
            {{
                add {{ this._RemoveEventEvent += value; }}
                remove {{ this._RemoveEventEvent -= value; }}
            }}
            
            {implementCode}
            
        }}
    }}
";

            return codeHeader;
        }

       

        

        private string _BuildGhostCode(IEnumerable<Type> types)
        {
            List<string> codes = new List<string>();
            foreach (Type type in types)
            {
                try
                {
                    string methods = _BuildMethods(type);
                    string propertys = _BuildPropertys(type);
                    string events = _BuildEvents(type);

                    codes.Add(methods);
                    codes.Add(propertys);
                    codes.Add(events);
                }
                catch(TypePropertyNotifierException tpne)
                {
                    throw new Exception(tpne.ToString());
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                            
                
            }
            return string.Join("\n", codes.ToArray());
        }

        private string _BuildEvents(Type type)
        {
            EventInfo[] eventinfos = type.GetEvents();
            List<string> codes = new List<string>();
            foreach (EventInfo eventinfo in eventinfos)
            {
                string code = $@"
                public Regulus.Remote.GhostEventHandler _{eventinfo.Name} = new Regulus.Remote.GhostEventHandler();
                event System.Action{_GetTypes(eventinfo.EventHandlerType.GetGenericArguments())} {_GetTypeName(type)}.{eventinfo.Name}
                {{
                    add {{ 
                            var id = _{eventinfo.Name}.Add(value);
                            _AddEventEvent(typeof({_GetTypeName(type)}).GetEvent(""{eventinfo.Name}""),id);
                        }}
                    remove {{ 
                                var id = _{eventinfo.Name}.Remove(value);
                                _RemoveEventEvent(typeof({_GetTypeName(type)}).GetEvent(""{eventinfo.Name}""),id);
                            }}
                }}
                ";
                codes.Add(code);
            }
            return string.Join("\n", codes.ToArray());
        }

        private string _GetTypes(Type[] generic_type_arguments)
        {
            IEnumerable<string> code = from t in generic_type_arguments select $"{_GetTypeName(t)}";
            if (code.Any())
                return "<" + string.Join(",", code.ToArray()) + ">";
            return "";
        }

        private string _BuildPropertys(Type type)
        {
            PropertyInfo[] propertyInfos = type.GetProperties();
            List<string> propertyCodes = new List<string>();
            _BuildRemoteProperty(type, propertyInfos, propertyCodes);
          
            return string.Join("\n", propertyCodes.ToArray());
        }

       

        private static bool _IsNotifierProperty(PropertyInfo propertyInfo)
        {
            try
            {
                if(propertyInfo.PropertyType.IsGenericType)
                    return propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(INotifier<>);
                return false;
            }
            catch (Exception ex)
            {
                throw new TypePropertyNotifierException(propertyInfo , ex);
            }
            
        }

        private void _BuildRemoteProperty(Type type, PropertyInfo[] propertyInfos, List<string> propertyCodes)
        {
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (!propertyInfo.PropertyType.GetInterfaces().Any(t => t == typeof(IDirtyable)))
                    continue;
                string propertyCode = $@"
                public {_GetTypeName(propertyInfo.PropertyType)} _{propertyInfo.Name}= new {_GetTypeName(propertyInfo.PropertyType)}();
                {_GetTypeName(propertyInfo.PropertyType)} {_GetTypeName(type)}.{propertyInfo.Name} {{ get{{ return _{propertyInfo.Name};}} }}";
                propertyCodes.Add(propertyCode);
            }
        }

        private string _GetTypeName(Type t)
        {

            if (!t.IsGenericType)
                return $"{t.Namespace}.{t.Name.ToString()}";
            StringBuilder sb = new StringBuilder();

            sb.Append(t.Name.Substring(0, t.Name.LastIndexOf("`")));
            sb.Append(t.GetGenericArguments().Aggregate("<",

                delegate (string aggregate, Type type)
                {
                    return aggregate + (aggregate == "<" ? "" : ",") + _GetTypeName(type);
                }
                ));
            sb.Append(">");


            return $"{t.Namespace}.{sb.ToString()}";


        }

        private string _BuildMethods(Type type)
        {

            List<string> methodCodes = new List<string>();
            MethodInfo[] methods = type.GetMethods();
            int id = 0;
            foreach (MethodInfo methodInfo in methods)
            {
                if (methodInfo.IsSpecialName)
                {
                    continue;
                }
                bool haveReturn = methodInfo.ReturnType != typeof(void);
                string returnTypeCode = "void";
                if (haveReturn)
                {
                    Type returnType = methodInfo.ReturnType.GetGenericArguments()[0];

                    returnTypeCode = $"Regulus.Remote.Value<{_GetTypeName(returnType)}>";
                }
                string returnValue = "";
                string addReturn = $"Regulus.Remote.IValue returnValue = null;";
                if (haveReturn)
                {
                    addReturn = $@"
    var returnValue = new {returnTypeCode}();
    
";
                    returnValue = "return returnValue;";
                }

                string addParams = _BuildAddParams(methodInfo);
                int paramId = 0;
                string paramCode = string.Join(",", (from p in methodInfo.GetParameters() select $"{ _GetTypeName(p.ParameterType)} {"_" + ++paramId}").ToArray());
                string methodCode = $@"
                {returnTypeCode} {type.Namespace}.{type.Name}.{methodInfo.Name}({paramCode})
                {{                    

                    {addReturn}
                    var info = typeof({methodInfo.DeclaringType}).GetMethod(""{methodInfo.Name}"");
                    _CallMethodEvent(info , new object[] {{{addParams}}} , returnValue);                    
                    {returnValue}
                }}

                
";
                methodCodes.Add(methodCode);
            }

            return string.Join(" \n", methodCodes.ToArray());
        }

        private string _BuildAddParams(MethodInfo method_info)
        {
            ParameterInfo[] parameters = method_info.GetParameters();

            List<string> addParams = new List<string>();

            for (int i = 0; i < parameters.Length; i++)
            {
                addParams.Add("_" + (i + 1));
            }
            return string.Join(",", addParams.ToArray());
        }
    }
}
