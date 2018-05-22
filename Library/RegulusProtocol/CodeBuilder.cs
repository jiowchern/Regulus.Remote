using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Regulus.Protocol
{
    public class CodeBuilder
    {
        static readonly string _GhostIdName = "_GhostIdName";

        

        public event Action<string, string> ProviderEvent;
        public event Action<string , string> GpiEvent;
        public event Action<string , string,string> EventEvent;


        
        public void Build(string protocol_name, Type[] types)
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
                

                if (type.IsInterface)
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
                            memberMapMethodBuilder.Add(String.Format("typeof({0}).GetMethod(\"{1}\")" , type.FullName , methodInfo.Name));
                    }


                    var propertyInfos = type.GetProperties();

                    foreach (var propertyInfo in propertyInfos)
                    {
                        
                            memberMapPropertyBuilder.Add(String.Format("typeof({0}).GetProperty(\"{1}\")", type.FullName, propertyInfo.Name));
                    }

                    memberMapInterfaceBuilder.Add(String.Format("typeof({0})", type.FullName));
                }
                

            }
            var addMemberMapinterfaceCode = string.Join(",", memberMapInterfaceBuilder.ToArray());
            var addMemberMapPropertyCode = string.Join(",", memberMapPropertyBuilder.ToArray());
            var addMemberMapEventCode = string.Join(",", memberMapEventBuilder.ToArray());
            
            var addMemberMapMethodCode = string.Join(",", memberMapMethodBuilder.ToArray());
            var addTypeCode = string.Join("\n", addGhostType.ToArray());
            var addDescriberCode = string.Join(",", _GetSerializarType(serializerTypes) );
            var addEventCode = string.Join("\n", addEventType.ToArray());
            var tokens = protocol_name.Split(new[] { '.' });
            var procotolName = tokens.Last();

            var providerNamespace = string.Join(".", tokens.Take(tokens.Count() - 1).ToArray());
            var providerNamespaceHead = "";
            var providerNamespaceTail = "";
            if (string.IsNullOrEmpty(providerNamespace) == false)
            {
                providerNamespaceHead = $"namespace {providerNamespace}{{ ";
                providerNamespaceTail = "}";
            }


            var builder = new StringBuilder();
            builder.Append(addTypeCode);
            builder.Append(addEventCode);
            builder.Append(addDescriberCode);
            
            var verificationCode = _BuildVerificationCode(builder);

            var providerCode =
                $@"
            using System;  
            using System.Collections.Generic;
            
            {providerNamespaceHead}
                public class {procotolName} : Regulus.Remoting.IProtocol
                {{
                    Regulus.Remoting.InterfaceProvider _InterfaceProvider;
                    Regulus.Remoting.EventProvider _EventProvider;
                    Regulus.Remoting.MemberMap _MemberMap;
                    Regulus.Serialization.ISerializer _Serializer;
                    public {procotolName}()
                    {{
                        var types = new Dictionary<Type, Type>();
                        {addTypeCode}
                        _InterfaceProvider = new Regulus.Remoting.InterfaceProvider(types);

                        var eventClosures = new List<Regulus.Remoting.IEventProxyCreator>();
                        {addEventCode}
                        _EventProvider = new Regulus.Remoting.EventProvider(eventClosures);

                        _Serializer = new Regulus.Serialization.Serializer(new Regulus.Serialization.DescriberBuilder({addDescriberCode}));


                        _MemberMap = new Regulus.Remoting.MemberMap(new System.Reflection.MethodInfo[] {{{addMemberMapMethodCode}}} ,new System.Reflection.EventInfo[]{{ {addMemberMapEventCode} }}, new System.Reflection.PropertyInfo[] {{{addMemberMapPropertyCode} }}, new System.Type[] {{{addMemberMapinterfaceCode}}});
                    }}

                    byte[] Regulus.Remoting.IProtocol.VerificationCode {{ get {{ return new byte[]{{{verificationCode}}};}} }}
                    Regulus.Remoting.InterfaceProvider Regulus.Remoting.IProtocol.GetInterfaceProvider()
                    {{
                        return _InterfaceProvider;
                    }}

                    Regulus.Remoting.EventProvider Regulus.Remoting.IProtocol.GetEventProvider()
                    {{
                        return _EventProvider;
                    }}

                    Regulus.Serialization.ISerializer Regulus.Remoting.IProtocol.GetSerialize()
                    {{
                        return _Serializer;
                    }}

                    Regulus.Remoting.MemberMap Regulus.Remoting.IProtocol.GetMemberMap()
                    {{
                        return _MemberMap;
                    }}
                    
                }}
            {providerNamespaceTail}
            ";

            if (ProviderEvent != null)
                ProviderEvent(protocol_name , providerCode);
        }

        private string _BuildVerificationCode(StringBuilder builder)
        {
            var md5 = MD5.Create();
            var code = md5.ComputeHash(Encoding.Default.GetBytes(builder.ToString()));            
            Regulus.Utility.Log.Instance.WriteInfo("Verification Code " + Convert.ToBase64String(code));
            return string.Join(",", code.Select(val => val.ToString()).ToArray());
        }

        private string[] _GetSerializarType(HashSet<Type> serializer_types)
        {
            var types = new HashSet<Type>();
            
            serializer_types.Add(typeof(Regulus.Remoting.PackageProtocolSubmit));
            serializer_types.Add(typeof(Regulus.Remoting.RequestPackage));
            serializer_types.Add(typeof(Regulus.Remoting.ResponsePackage));
            serializer_types.Add(typeof(Regulus.Remoting.PackageUpdateProperty));
            serializer_types.Add(typeof(Regulus.Remoting.PackageInvokeEvent));
            serializer_types.Add(typeof(Regulus.Remoting.PackageErrorMethod));
            serializer_types.Add(typeof(Regulus.Remoting.PackageReturnValue));
            serializer_types.Add(typeof(Regulus.Remoting.PackageLoadSoulCompile));
            serializer_types.Add(typeof(Regulus.Remoting.PackageLoadSoul));
            serializer_types.Add(typeof(Regulus.Remoting.PackageUnloadSoul));
            serializer_types.Add(typeof(Regulus.Remoting.PackageCallMethod));
            serializer_types.Add(typeof(Regulus.Remoting.PackageRelease));

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

            return $"{type.Namespace}.Event.{type.Name}.{event_name}";
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
    
    namespace {nameSpace}.Event.{name} 
    {{ 
        public class {eventName} : Regulus.Remoting.IEventProxyCreator
        {{

            Type _Type;
            string _Name;
            
            public {eventName}()
            {{
                _Name = ""{eventName}"";
                _Type = typeof({type.FullName});                   
            
            }}
            Delegate Regulus.Remoting.IEventProxyCreator.Create(Guid soul_id,int event_id, Regulus.Remoting.InvokeEventCallabck invoke_Event)
            {{                
                var closure = new Regulus.Remoting.GenericEventClosure{_GetTypes(argTypes)}(soul_id , event_id , invoke_Event);                
                return new Action{_GetTypes(argTypes)}(closure.Run);
            }}
        

            Type Regulus.Remoting.IEventProxyCreator.GetType()
            {{
                return _Type;
            }}            

            string Regulus.Remoting.IEventProxyCreator.GetName()
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
        public class C{name} : {_GetTypeName(type)} , Regulus.Remoting.IGhost
        {{
            readonly bool _HaveReturn ;
            
            readonly Guid {CodeBuilder._GhostIdName};
            
            
            
            public C{name}(Guid id, bool have_return )
            {{
                _HaveReturn = have_return ;
                {CodeBuilder._GhostIdName} = id;            
            }}
            

            Guid Regulus.Remoting.IGhost.GetID()
            {{
                return {CodeBuilder._GhostIdName};
            }}

            bool Regulus.Remoting.IGhost.IsReturnType()
            {{
                return _HaveReturn;
            }}
            object Regulus.Remoting.IGhost.GetInstance()
            {{
                return this;
            }}

            private event Regulus.Remoting.CallMethodCallback _CallMethodEvent;

            event Regulus.Remoting.CallMethodCallback Regulus.Remoting.IGhost.CallMethodEvent
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
                System.Action{_GetTypes(eventinfo.EventHandlerType.GetGenericArguments())} _{eventinfo.Name};
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
                {_GetTypeName(propertyInfo.PropertyType)} _{propertyInfo.Name};
                {_GetTypeName(propertyInfo.PropertyType)} {_GetTypeName(type)}.{propertyInfo.Name} {{ get{{ return _{propertyInfo.Name};}} }}";
                propertyCodes.Add(propertyCode);
            }
            return string.Join("\n", propertyCodes.ToArray());
        }

        private string _GetTypeName(Type type)
        {

            return type.FullName.Replace("+", ".");
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

                    returnTypeCode = $"Regulus.Remoting.Value<{_GetTypeName(returnType)}>";
                }
                var returnValue = "";
                var addReturn = $"Regulus.Remoting.IValue returnValue = null;";
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
            return string.Join(" ,", addParams.ToArray());
        }
    }
}
