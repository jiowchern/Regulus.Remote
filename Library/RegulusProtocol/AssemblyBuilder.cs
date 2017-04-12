using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.CSharp;

namespace Regulus.Protocol
{
    public class AssemblyBuilder
    {
        static string ghostIdName = "_GhostIdName";
        

        public IEnumerable<string> Build(string path,string output_path,string library_name, string[] namesapces)
        {
            byte[] sourceDll = System.IO.File.ReadAllBytes(path);
            Assembly assembly = Assembly.Load(sourceDll);
            var types = assembly.GetExportedTypes();

            var codes = Build(library_name, namesapces, types);

            Dictionary<string, string> optionsDic = new Dictionary<string, string>
            {
                {"CompilerVersion", "v3.5"}
            };
            var provider = new CSharpCodeProvider(optionsDic);
            var options = new CompilerParameters
            {
                
                OutputAssembly = output_path,
                GenerateExecutable = false,
                ReferencedAssemblies =
                {
                    "System.Core.dll",
                    "RegulusLibrary.dll",
                    "RegulusRemoting.dll",
                    "protobuf-net.dll",
                    path,
                }
            };
            var result = provider.CompileAssemblyFromSource(options, codes.ToArray());

            return codes;
        }

        public IEnumerable<string> Build(string library_name, string[] namesapces, Type[] types)
        {
            var codes = new List<string>();

            List<string> addGhostType = new List<string>();
            List<string> addEventType = new List<string>();

            foreach (var type in types)
            {
                if (namesapces.Any(n => n == type.Namespace) && type.IsInterface)
                {
                    string ghostClassCode = _BuildGhostCode(type);
                    addGhostType.Add($"types.Add(typeof({_GetTypeName(type)}) , typeof({_GetGhostType(type)}) );");
                    codes.Add(ghostClassCode);
                    

                    var eventInfos = type.GetEvents();
                    foreach (var eventInfo in eventInfos)
                    {
                        addEventType.Add($"eventClosures.Add(new {_GetEventType(type, eventInfo.Name)}() );");
                        codes.Add(_BuildEventCode(type , eventInfo));
                    }
                    
                }
            }
            var addTypeCode = string.Join("\n", addGhostType);

            var addEventCode = string.Join("\n", addEventType);
            var tokens = library_name.Split(new[]{'.'});
            var procotolName = tokens.Last();

            var providerNamespace = string.Join(".", tokens.Take(tokens.Count() - 1));
            var providerNamespaceHead = "";
            var providerNamespaceTail = "";
            if (string.IsNullOrWhiteSpace(providerNamespace) == false)
            {
                providerNamespaceHead = $"namespace {providerNamespace}{{ ";
                providerNamespaceTail = "}";
            }
            var providerCode =
                $@"
            using System;  
            using System.Collections.Generic;
            
            {providerNamespaceHead}
                public class {procotolName} : Regulus.Remoting.IProtocol
                {{
                    Regulus.Remoting.GPIProvider _GPIProvider;
                    Regulus.Remoting.EventProvider _EventProvider;
                    public {procotolName}()
                    {{
                        var types = new Dictionary<Type, Type>();
                        {addTypeCode}
                        _GPIProvider = new Regulus.Remoting.GPIProvider(types);

                        var eventClosures = new List<Regulus.Remoting.IEventProxyCreator>();
                        {addEventCode}
                        _EventProvider = new Regulus.Remoting.EventProvider(eventClosures);
                    }}


                    Regulus.Remoting.GPIProvider Regulus.Remoting.IProtocol.GetGPIProvider()
                    {{
                        return _GPIProvider;
                    }}

                    Regulus.Remoting.EventProvider Regulus.Remoting.IProtocol.GetEventProvider()
                    {{
                        return _EventProvider;
                    }}
                    
                }}
            {providerNamespaceTail}
            ";

            codes.Add(providerCode);
            

            return codes;
        }

        
        

        private string _GetGhostType(Type type)
        {
            return $"{type.Namespace}.Ghost.C{type.Name}";
        }

        private string _GetEventType(Type type,string event_name)
        {
            
            return $"{type.Namespace}.Event.{type.Name}.{event_name}";
        }


        private static bool _IsGPI(Type type)
        {
            return true;
        }
        private string  _BuildEventCode(Type type , EventInfo  info)
        {
            var nameSpace = type.Namespace;
            var name = type.Name;
            
            var argTypes = info.EventHandlerType.GenericTypeArguments;
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
            Delegate Regulus.Remoting.IEventProxyCreator.Create(Guid soul_id, Action<Guid, string, object[]> invoke_Event)
            {{                
                var closure = new Regulus.Remoting.GenericEventClosure{_GetTypes(argTypes)}(soul_id , _Name , invoke_Event);                
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
            bool _HaveReturn ;
            Regulus.Remoting.IGhostRequest _Requester;
            Guid {AssemblyBuilder.ghostIdName};
            Regulus.Remoting.ReturnValueQueue _Queue;
            public C{name}(Regulus.Remoting.IGhostRequest requester , Guid id,Regulus.Remoting.ReturnValueQueue queue, bool have_return )
            {{
                _Requester = requester;
                _HaveReturn = have_return ;
                {AssemblyBuilder.ghostIdName} = id;
                _Queue = queue;
            }}

            void Regulus.Remoting.IGhost.OnEvent(string name_event, byte[][] args)
            {{
                Regulus.Remoting.AgentCore.CallEvent(name_event , ""C{name}"" , this , args);
            }}

            Guid Regulus.Remoting.IGhost.GetID()
            {{
                return {AssemblyBuilder.ghostIdName};
            }}

            bool Regulus.Remoting.IGhost.IsReturnType()
            {{
                return _HaveReturn;
            }}

            void Regulus.Remoting.IGhost.OnProperty(string name, byte[] value)
            {{
                Regulus.Remoting.AgentCore.UpdateProperty(name , ""C{name}"" , this , value);
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
            return string.Join("\n", codes);
        }

        private string _BuildEvents(Type type)
        {
            var eventinfos = type.GetEvents();
            var codes = new List<string>();
            foreach (var eventinfo in eventinfos)
            {                
                var code = $@"
                System.Action{_GetTypes(eventinfo.EventHandlerType.GenericTypeArguments)} _{eventinfo.Name};
                event System.Action{_GetTypes(eventinfo.EventHandlerType.GenericTypeArguments)} {_GetTypeName(type)}.{eventinfo.Name}
                {{
                    add {{ _{eventinfo.Name} += value;}}
                    remove {{ _{eventinfo.Name} -= value;}}
                }}
                ";
                codes.Add(code);
            }
            return string.Join("\n", codes);
        }

        private string _GetTypes(Type[] generic_type_arguments)
        {
            var code = from t in generic_type_arguments select $"{_GetTypeName(t)}";
            if (code.Any())
                return "<" + string.Join(",", code) + ">";
            return "";
        }

        private string _BuildPropertys(Type type)
        {
            var propertyInfos = type.GetProperties();
            List<string> propertyCodes = new List<string>();
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyType = propertyInfo.PropertyType;
                var propertyCode = $@"
                {_GetTypeName(propertyInfo.PropertyType)} _{propertyInfo.Name};
                {_GetTypeName(propertyInfo.PropertyType)} {_GetTypeName(type)}.{propertyInfo.Name} {{ get{{ return _{propertyInfo.Name};}} }}";
                propertyCodes.Add(propertyCode);
            }
            return string.Join("\n", propertyCodes);
        }

        private string _GetTypeName(Type type)
        {
            
            return type.FullName.Replace("+", ".");
        }

        private string _BuildMethods(Type type)
        {
            
            List<string> methodCodes = new List<string>();
            var methods = type.GetMethods();
            foreach (var methodInfo in methods)
            {
                if (methodInfo.IsSpecialName)
                {
                    continue;
                }
                bool haveReturn = methodInfo.ReturnType != typeof (void);
                var returnTypeCode = "void";
                if (haveReturn)
                {
                    var returnType = methodInfo.ReturnType.GenericTypeArguments[0];
                    
                    returnTypeCode = $"Regulus.Remoting.Value<{_GetTypeName(returnType)}>";
                }
                var returnValue = "";
                var addReturn = $"";
                if (haveReturn)
                {
                    addReturn = $@"
    var returnValue = new {returnTypeCode}();
    var returnId = _Queue.PushReturnValue(returnValue);    
    data.ReturnId = returnId;
";
                    returnValue = "return returnValue;";
                }

                var addParams = _BuildAddParams(methodInfo);

                var paramCode = string.Join(",", (from p in methodInfo.GetParameters() select $"{ _GetTypeName(p.ParameterType)} {p.Name}"));
                var methodCode = $@"
                {returnTypeCode} {type.Namespace}.{type.Name}.{methodInfo.Name}({paramCode})
                {{                    

                        
                    var data = new Regulus.Remoting.PackageCallMethod();
                    data.EntityId = {AssemblyBuilder.ghostIdName};
                    data.MethodName =""{methodInfo.Name}"";
                    {addReturn}
                    {addParams}
                    _Requester.Request(Regulus.Remoting.ClientToServerOpCode.CallMethod , data.ToBuffer());

                    {returnValue}
                }}
";
                methodCodes.Add(methodCode);
            }

            return string.Join(" \n" , methodCodes);
        }

        private string _BuildAddParams(MethodInfo method_info)
        {
            var parameters = method_info.GetParameters();            

            List<string> addParams = new List<string>();
            
            string addParamsHead = @"var paramList = new System.Collections.Generic.List<byte[]>();";
            foreach (var parameterInfo in parameters)
            {
                var addparam = $@"
    var {parameterInfo.Name}Bytes = Regulus.TypeHelper.Serializer<{parameterInfo.ParameterType.Namespace}.{parameterInfo.ParameterType.Name}>({parameterInfo.Name});    
    paramList.Add({parameterInfo.Name}Bytes);
";

                
                addParams.Add(addparam);
            }

            return $"{addParamsHead}\n{string.Join(" \n", addParams)}\ndata.MethodParams = paramList.ToArray();";
        }
    }
}
