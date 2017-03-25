using System;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.CodeDom.Compiler;
using System.Runtime.Remoting.Messaging;

using Microsoft.CSharp;
namespace Regulus.Tool
{
    public class GhostProviderGenerator
    {
        static string ghostIdName = "_GhostIdName";

        public class Output
        {
            public IEnumerable<string> Codes;

            public CompilerResults Result;
        }
        public Output BuildProvider(string path,string output_path,string provider_name, string[] namesapces)
        {
            byte[] sourceDll = System.IO.File.ReadAllBytes(path);
            Assembly assembly = Assembly.Load(sourceDll);
            var types = assembly.GetExportedTypes();

            var codes = BuildProvider(provider_name, namesapces, types);

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

            return new Output
            {
                Result = result,
                Codes = codes
            };
        }

        public IEnumerable<string> BuildProvider(string provider_name, string[] namesapces, Type[] types)
        {
            var codes = new List<string>();

            List<string> addTypes = new List<string>();
            foreach (var type in types)
            {
                if (namesapces.Any(n => n == type.Namespace) && type.IsInterface
                    && GhostProviderGenerator._IsGPI(type))
                {
                    string code = _BuildCode(type);
                    codes.Add(code);
                    addTypes.Add($"_Types.Add(typeof({_GetTypeName(type)}) , typeof({_GetGhostType(type)}) );");
                }
            }
            var addTypeCode = string.Join("\n", addTypes);
            var tokens = provider_name.Split(
                new[]
                {
                    '.'
                });
            var providerName = tokens.Last();

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
            
            {
                    providerNamespaceHead}
                public class {providerName
                    } : Regulus.Remoting.IGhostProvider
                {{
                    private Dictionary<Type, Type> _Types ;
                    public {
                    providerName
                    }()
                    {{
                        _Types = new Dictionary<Type, Type>();
                        {
                            addTypeCode
                    }
                    }}
                    Type Regulus.Remoting.IGhostProvider.Find(Type ghost_base_type)
                    {{
                        if(_Types.ContainsKey(ghost_base_type))                                   
                        {{
                            return _Types[ghost_base_type];
                        }}
                        return null;
                    }}
                }}
            {
                    providerNamespaceTail}
            ";

            codes.Add(providerCode);
            

            return codes;
        }

        private string _GetGhostType(Type type)
        {
            return $"{type.Namespace}.Ghost.C{type.Name}";
        }

        private static bool _IsGPI(Type type)
        {
            return true;
        }

        private string _BuildCode(Type type)
        {
            var nameSpace = type.Namespace;
            
            var name = type.Name;

            
            var types = type.GetInterfaces().Concat(
                new[]
                {
                    type
                });
            string implementCode = _BuildCode(types);
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
            Guid {ghostIdName};
            Regulus.Remoting.ReturnValueQueue _Queue;
            public C{name}(Regulus.Remoting.IGhostRequest requester , Guid id,Regulus.Remoting.ReturnValueQueue queue, bool have_return )
            {{
                _Requester = requester;
                _HaveReturn = have_return ;
                {ghostIdName} = id;
                _Queue = queue;
            }}

            void Regulus.Remoting.IGhost.OnEvent(string name_event, byte[][] args)
            {{
                Regulus.Remoting.AgentCore.CallEvent(name_event , ""C{name}"" , this , args);
            }}

            Guid Regulus.Remoting.IGhost.GetID()
            {{
                return {ghostIdName};
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

        private string _BuildCode(IEnumerable<Type> types)
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
                    data.EntityId = {ghostIdName};
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
