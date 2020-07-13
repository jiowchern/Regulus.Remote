using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Regulus.Remote.Protocol
{
    public class CodeBuilder
    {
        static readonly string _GhostIdName = "_GhostIdName";

        

        public event Action<string, string> ProviderEvent;
        public event Action<string , string> GpiEvent;
        public event Action<string , string,string> EventEvent;


        
        public void Build(Type[] types)
        {

            var codeGpis = new List<string>();
            var codeEvents = new List<string>();


            var addGhostType = new List<string>();
            var addEventType = new List<string>();

            var serializerTypes = new HashSet<Type>();
            
            var memberMapMethodBuilder = new List<string>();
            var memberMapEventBuilder = new List<string>();
            var memberMapPropertyBuilder = new List<string>();

            var memberMapInterfaceBuilder = new List<string>();
            foreach (var type in types)
            {
                
                serializerTypes.Add(type);
                

                if (_ValidRemoteInterface(type))
                {
                    string ghostClassCode = _BuildGhostCode(type);
                    var typeName = _GetTypeName(type);
                    addGhostType.Add($"types.Add(typeof({typeName}) , typeof({_GetGhostType(type)}) );");
                    codeGpis.Add(ghostClassCode);
                    if (GpiEvent != null)
                        GpiEvent(typeName, ghostClassCode);

                    var eventInfos = type.GetEvents();
                    foreach (var eventInfo in eventInfos)
                    {
                        addEventType.Add($"eventClosures.Add(new {_GetEventType(type, eventInfo.Name)}() );");
                        var eventCode = _BuildEventCode(type, eventInfo);
                        codeEvents.Add(eventCode);

                        if (EventEvent != null)
                            EventEvent(typeName, eventInfo.Name, eventCode);

                        
                        memberMapEventBuilder.Add(String.Format("typeof({0}).GetEvent(\"{1}\")", type.FullName, eventInfo.Name));
                    }

                    var methodInfos = type.GetMethods();
                    foreach (var methodInfo in methodInfos)
                    {
                        if (methodInfo.IsPublic && methodInfo.IsSpecialName == false)
                        {
                            //String.Format("typeof({0}).GetMethod(\"{1}\")", type.FullName, methodInfo.Name)
                            memberMapMethodBuilder.Add(_BuildGetTypeMethodInfo(methodInfo));
                        }                            
                    }


                    var propertyInfos = type.GetProperties();

                    foreach (var propertyInfo in propertyInfos)
                    {
                        
                            memberMapPropertyBuilder.Add(String.Format("typeof({0}).GetProperty(\"{1}\")", type.FullName, propertyInfo.Name));
                    }

                    memberMapInterfaceBuilder.Add(String.Format("new System.Tuple<System.Type, System.Func<Regulus.Remote.IProvider>>(typeof({0}),()=>new Regulus.Remote.TProvider<{0}>())", type.FullName));
                }
                

            }
            var addMemberMapinterfaceCode = string.Join(",", memberMapInterfaceBuilder.ToArray());
            var addMemberMapPropertyCode = string.Join(",", memberMapPropertyBuilder.ToArray());
            var addMemberMapEventCode = string.Join(",", memberMapEventBuilder.ToArray());
            
            var addMemberMapMethodCode = string.Join(",", memberMapMethodBuilder.ToArray());
            var addTypeCode = string.Join("\n", addGhostType.ToArray());
            var addDescriberCode = string.Join(",", _GetSerializarType(serializerTypes) );
            var addEventCode = string.Join("\n", addEventType.ToArray());
            //var tokens = protocol_name.Split(new[] { '.' });
            //var procotolName = tokens.Last();

           // var providerNamespace = string.Join(".", tokens.Take(tokens.Count() - 1).ToArray());
            var providerNamespaceHead = "";
            var providerNamespaceTail = "";
            /*if (string.IsNullOrEmpty(providerNamespace) == false)
            {
                providerNamespaceHead = $"namespace {providerNamespace}{{ ";
                providerNamespaceTail = "}";
            }*/


            var builder = new StringBuilder();
            builder.Append(addTypeCode);
            builder.Append(addEventCode);
            builder.Append(addDescriberCode);

            var md5 = _BuildMd5(builder);
            var verificationCode = _BuildVerificationCode(md5);
            var procotolName = _BuildProtocolName(md5);
            var providerCode =
                $@"
            using System;  
            using System.Collections.Generic;
            
            {providerNamespaceHead}
                public class {procotolName} : Regulus.Remote.IProtocol
                {{
                    Regulus.Remote.InterfaceProvider _InterfaceProvider;
                    Regulus.Remote.EventProvider _EventProvider;
                    Regulus.Remote.MemberMap _MemberMap;
                    Regulus.Serialization.ISerializer _Serializer;
                    public {procotolName}()
                    {{
                        var types = new Dictionary<Type, Type>();
                        {addTypeCode}
                        _InterfaceProvider = new Regulus.Remote.InterfaceProvider(types);

                        var eventClosures = new List<Regulus.Remote.IEventProxyCreator>();
                        {addEventCode}
                        _EventProvider = new Regulus.Remote.EventProvider(eventClosures);

                        _Serializer = new Regulus.Serialization.Serializer(new Regulus.Serialization.DescriberBuilder({addDescriberCode}).Describers);


                        _MemberMap = new Regulus.Remote.MemberMap(new System.Reflection.MethodInfo[] {{{addMemberMapMethodCode}}} ,new System.Reflection.EventInfo[]{{ {addMemberMapEventCode} }}, new System.Reflection.PropertyInfo[] {{{addMemberMapPropertyCode} }}, new System.Tuple<System.Type, System.Func<Regulus.Remote.IProvider>>[] {{{addMemberMapinterfaceCode}}});
                    }}

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
                ProviderEvent(procotolName , providerCode);
        }

        private bool _ValidRemoteInterface(Type type)
        {
            
            return type.IsInterface;
        }

        internal string _BuildGetTypeMethodInfo(MethodInfo method_info)
        {
            
            var methodCode = method_info.Name;
            var argTypes = method_info.GetParameters().Select(p => p.ParameterType);
            var argTypesCode = _GetTypes(new []{ method_info.DeclaringType }.Concat(argTypes).ToArray() );
            var argInstanceCode = method_info.GetParameters().Length > 0 ? "ins," : "ins";
            
            var paramCode = _BuildAddParams(method_info);
            
            var code =
                $"new Regulus.Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<System.Action{argTypesCode}>)(({argInstanceCode}{paramCode}) => ins.{methodCode}({paramCode}))).Method";
            return code;
        }

        private byte[] _BuildMd5(StringBuilder builder)
        {
            var md5 = MD5.Create();
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
            var types = new HashSet<Type>();
            
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

            foreach (var serializerType in serializer_types)
            {                
                foreach (var type in new TypeDisintegrator(serializerType).Types)
                {                    
                    types.Add(type);
                }
            }
            var typeCodes = (from type in types orderby type.FullName select "typeof(" + _GetTypeName(type) + ")").ToArray();

            foreach (var type in types)
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
            var nameSpace = type.Namespace;
            var name = type.Name;

            var argTypes = info.EventHandlerType.GetGenericArguments();
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
            Delegate Regulus.Remote.IEventProxyCreator.Create(long soul_id,int event_id, Regulus.Remote.InvokeEventCallabck invoke_Event)
            {{                
                var closure = new Regulus.Remote.GenericEventClosure{_GetTypes(argTypes)}(soul_id , event_id , invoke_Event);                
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
            var nameSpace = type.Namespace;

            var name = type.Name;

            var types = type.GetInterfaces().Concat(
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
            
            {implementCode}
            
        }}
    }}
";

            return codeHeader;
        }

        private string _BuildGhostCode(IEnumerable<Type> types)
        {
            List<string> codes = new List<string>();
            foreach (var type in types)
            {
                string methods = _BuildMethods(type);
                string propertys = _BuildPropertys(type);
                string events = _BuildEvents(type);

                codes.Add(methods);
                codes.Add(propertys);
                codes.Add(events);
            }
            return string.Join("\n", codes.ToArray());
        }

        private string _BuildEvents(Type type)
        {
            var eventinfos = type.GetEvents();
            var codes = new List<string>();
            foreach (var eventinfo in eventinfos)
            {
                var code = $@"
                public System.Action{_GetTypes(eventinfo.EventHandlerType.GetGenericArguments())} _{eventinfo.Name};
                event System.Action{_GetTypes(eventinfo.EventHandlerType.GetGenericArguments())} {_GetTypeName(type)}.{eventinfo.Name}
                {{
                    add {{ _{eventinfo.Name} += value;}}
                    remove {{ _{eventinfo.Name} -= value;}}
                }}
                ";
                codes.Add(code);
            }
            return string.Join("\n", codes.ToArray());
        }

        private string _GetTypes(Type[] generic_type_arguments)
        {
            var code = from t in generic_type_arguments select $"{_GetTypeName(t)}";
            if (code.Any())
                return "<" + string.Join(",", code.ToArray()) + ">";
            return "";
        }

        private string _BuildPropertys(Type type)
        {
            var propertyInfos = type.GetProperties();
            List<string> propertyCodes = new List<string>();
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyCode = $@"
                public {_GetTypeName(propertyInfo.PropertyType)} _{propertyInfo.Name}= new {_GetTypeName(propertyInfo.PropertyType)}();
                {_GetTypeName(propertyInfo.PropertyType)} {_GetTypeName(type)}.{propertyInfo.Name} {{ get{{ return _{propertyInfo.Name};}} }}";
                propertyCodes.Add(propertyCode);
            }
            return string.Join("\n", propertyCodes.ToArray());
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
            var methods = type.GetMethods();
            int id = 0;
            foreach (var methodInfo in methods)
            {
                if (methodInfo.IsSpecialName)
                {
                    continue;
                }
                bool haveReturn = methodInfo.ReturnType != typeof(void);
                var returnTypeCode = "void";
                if (haveReturn)
                {
                    var returnType = methodInfo.ReturnType.GetGenericArguments()[0];

                    returnTypeCode = $"Regulus.Remote.Value<{_GetTypeName(returnType)}>";
                }
                var returnValue = "";
                var addReturn = $"Regulus.Remote.IValue returnValue = null;";
                if (haveReturn)
                {
                    addReturn = $@"
    var returnValue = new {returnTypeCode}();
    
";
                    returnValue = "return returnValue;";
                }

                var addParams = _BuildAddParams(methodInfo);
                int paramId = 0;
                var paramCode = string.Join(",", (from p in methodInfo.GetParameters() select $"{ _GetTypeName(p.ParameterType)} {"_" + ++paramId}").ToArray());
                var methodCode = $@"
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
            var parameters = method_info.GetParameters();

            List<string> addParams = new List<string>();

            for (int i = 0; i < parameters.Length; i++)
            {
                addParams.Add("_" + (i+1));
            }
            return string.Join(",", addParams.ToArray());
        }
    }
}
