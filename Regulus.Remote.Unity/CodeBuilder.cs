using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regulus.Remote.Unity
{

    public class ProtocolGenerator
    {

    }
    public class CodeBuilder  
    {
        private readonly Type[] _Types;

        private readonly string _ProviderName;

        private string _AgentClassName;

        private string _NamespaceHead;

        private string _NamespaceTail;

        private string _Namespace;

        public event System.Action<string,string> AgentEvent;

        public event System.Action<string , string , string> TypeEvent;

        public CodeBuilder(Type[] types, string agent_name , string provider_name)
        {
            
            _Types = (from type in types where  type.IsInterface == true select type).ToArray();
            _ProviderName = provider_name;

            var tokens = agent_name.Split(new[] { '.' });
            _AgentClassName = tokens.Last();

            
            _NamespaceHead = "";
            _NamespaceTail = "";
            _Namespace = string.Join(".", tokens.Take(tokens.Count() - 1).ToArray());
            if (string.IsNullOrEmpty(_Namespace) == false)
            {
                _NamespaceHead = $"namespace {_Namespace}{{ ";
                _NamespaceTail = "}";
            }
        }

        public void Build()
        {

            if (TypeEvent != null)
                foreach (var type in _Types)
                {
                    TypeEvent( _GetClassName(type.Name), _BuildAdsorberCode(type), _BuildBroadcasterCode(type));
                    // TODO : _BuildInspectorCode(type);
                }

            if (AgentEvent != null)
                AgentEvent(_AgentClassName, _BuildAgentCode());
        }
        
        private string _BuildAgentCode()
        {
            return string.Format(@"
using System;

using Regulus.Utility;

using UnityEngine;


{0}
    public abstract class {3} : MonoBehaviour
    {{
        private Regulus.Remote.Unity.Distributor _Distributor ;
        public Regulus.Remote.Unity.Distributor Distributor {{ get{{ return _Distributor ; }} }}
        private readonly Regulus.Utility.Updater _Updater;

        private Regulus.Remote.IAgent _Agent;
        public string Name;
        public {3}()
        {{            
            _Updater = new Updater();
        }}
        public abstract Regulus.Remote.IAgent _GetAgent();
        void Start()   
        {{
            _Agent = _GetAgent();
            _Distributor  = new Regulus.Remote.Unity.Distributor(_Agent);
            _Updater.Add(_Agent);
        }}
        public void Connect(string ip,int port)
        {{            
            _Agent.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), port)).OnValue += _ConnectResult;
        }}
        
        

        private void _ConnectResult(bool obj)
        {{
            ConnectEvent.Invoke(obj);
        }}

        void OnDestroy()
        {{
            _Updater.Shutdown();
        }}

       
        void Update()
        {{
            _Updater.Working();
        }}
        [Serializable]
        public class UnityAgentConnectEvent : UnityEngine.Events.UnityEvent<bool>{{}}

        public UnityAgentConnectEvent ConnectEvent;
    }}
{1}
", _NamespaceHead, _NamespaceTail ,_ProviderName , _AgentClassName);
        }
        private string _BuildBroadcasterCode(Type type)
        {
            return string.Format(@"
using System;

using System.Linq;

using Regulus.Utility;

using UnityEngine;
using UnityEngine.Events;

{3}
    public class {2}Broadcaster : UnityEngine.MonoBehaviour 
    {{
        public string Agent;        
        Regulus.Remote.INotifier<{0}.{1}> _Notifier;

        private readonly Regulus.Utility.StageMachine _Machine;

        public {2}Broadcaster()
        {{
            _Machine = new StageMachine();
        }} 

        void Start()
        {{
            _ToScan();
        }}

        private void _ToScan()
        {{
            var stage = new Regulus.Utility.SimpleStage(_ScanEnter , _ScaneLeave , _ScaneUpdate);

            _Machine.Push(stage);
        }}


        private void _ScaneUpdate()
        {{
            var agents = GameObject.FindObjectsOfType<{5}.{6}>();
            var agent = agents.FirstOrDefault(d => d.Name == Agent);
            if (agent != null)
            {{
                _Notifier = agent.Distributor.QueryNotifier<{0}.{1}>();

                _ToInitial();                                
            }}
        }}

        private void _ToInitial()
        {{
            var stage = new Regulus.Utility.SimpleStage(_Initial);
            _Machine.Push(stage);
        }}

        private void _Initial()
        {{
            _Notifier.Supply += _Supply;
            _Notifier.Unsupply += _Unsupply;
        }}

        private void _ScaneLeave()
        {{
            
        }}

        private void _ScanEnter()
        {{
            
        }}

        // Update is called once per frame
        void Update()
        {{
            _Machine.Update();
        }}

        void OnDestroy()
        {{
            if (_Notifier != null)
            {{
                _Notifier.Supply -= _Supply;
                _Notifier.Unsupply -= _Unsupply;
            }}
                
            _Machine.Termination();
        }}

        private void _Unsupply({0}.{1} obj)
        {{
            UnsupplyEvent.Invoke(obj);
        }}

        private void _Supply({0}.{1} obj)
        {{
            SupplyEvent.Invoke(obj);
        }}

        [Serializable]
        public class UnityBroadcastEvent : UnityEvent<{0}.{1}>{{}}

        public UnityBroadcastEvent SupplyEvent;
        public UnityBroadcastEvent UnsupplyEvent;
    }}
{4}
", type.Namespace , type.Name, _GetClassName(type.Name) , _NamespaceHead , _NamespaceTail, _Namespace , _AgentClassName);
        }
        private string _BuildAdsorberCode(Type type)
        {
            var code = string.Format(
                @"                    

namespace {0}.Adsorption
{{
    using System.Linq;
        
    public class {7}Adsorber : UnityEngine.MonoBehaviour , Regulus.Remote.Unity.Adsorber<{1}>
    {{
        private readonly Regulus.Utility.StageMachine _Machine;        
        
        public string Agent;

        private global::{8}.{9} _Agent;

        [System.Serializable]
        public class UnityEnableEvent : UnityEngine.Events.UnityEvent<bool> {{}}
        public UnityEnableEvent EnableEvent;
        [System.Serializable]
        public class UnitySupplyEvent : UnityEngine.Events.UnityEvent<{0}.{1}> {{}}
        public UnitySupplyEvent SupplyEvent;
        {0}.{1} _{7};                        
       
        public {7}Adsorber()
        {{
            _Machine = new Regulus.Utility.StageMachine();
        }}

        void Start()
        {{
            _Machine.Push(new Regulus.Utility.SimpleStage(_ScanEnter, _ScanLeave, _ScanUpdate));
        }}

        private void _ScanUpdate()
        {{
            var agents = UnityEngine.GameObject.FindObjectsOfType<global::{8}.{9}>();
            _Agent = agents.FirstOrDefault(d => string.IsNullOrEmpty(d.Name) == false && d.Name == Agent);
            if(_Agent != null)
            {{
                _Machine.Push(new Regulus.Utility.SimpleStage(_DispatchEnter, _DispatchLeave));
            }}            
        }}

        private void _DispatchEnter()
        {{
            _Agent.Distributor.Attach<{1}>(this);
        }}

        private void _DispatchLeave()
        {{
            _Agent.Distributor.Detach<{1}>(this);
        }}

        private void _ScanLeave()
        {{

        }}


        private void _ScanEnter()
        {{

        }}

        void Update()
        {{
            _Machine.Update();
        }}

        void OnDestroy()
        {{
            _Machine.Termination();
        }}

        public {0}.{1} GetGPI()
        {{
            return _{7};
        }}
        public void Supply({0}.{1} gpi)
        {{
            _{7} = gpi;
            {5}
            EnableEvent.Invoke(true);
            SupplyEvent.Invoke(gpi);
        }}

        public void Unsupply({0}.{1} gpi)
        {{
            EnableEvent.Invoke(false);
            {6}
            _{7} = null;
        }}
        {2}
        {3}
        {4}
    }}
}}
                    ", type.Namespace, type.Name, _GenerateMethods(type), _GenerateReturnEvents(type),
                _GenerateEvents(type), _GetBindEvents(type, "+="), _GetBindEvents(type, "-="), _GetClassName(type.Name),_Namespace , _AgentClassName);


            return code;
        }

        private object _GetBindEvents(Type type, string op_code)
        {
            var codes = new List<string>();
            foreach (var eventInfo in type.GetEvents())
            {
                codes.Add(_GetBindEvent(type, eventInfo, op_code));
            }
            return string.Join("\n", codes.ToArray());
        }

        private string _GetBindEvent(Type type, EventInfo event_info, string op_code)
        {

            return string.Format("_{0}.{1} {2} _On{1};", _GetClassName(type.Name), event_info.Name, op_code);
        }

        private string _GenerateReturnEvents(Type type)
        {
            var methodInfos = type.GetMethods();
            var codes = new List<string>();
            foreach (var methodInfo in methodInfos)
            {
                if (methodInfo.IsSpecialName)
                {
                    continue;
                }

                var returnType = methodInfo.ReturnType;

                if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Regulus.Remote.Value<>))
                {
                    codes.Add(_GenerateMethodReturnEvent(methodInfo));
                }
            }

            return string.Join("\n", codes.ToArray());
        }

        private string _GenerateMethodReturnEvent(MethodInfo method_info)
        {
            var genericArguments = method_info.ReturnType.GetGenericArguments();
            var argTypes = _GetArgTypes(genericArguments);
            string code = String.Format(@"
        [System.Serializable]
        public class Unity{0}Result : UnityEngine.Events.UnityEvent{1} {{ }}
        public Unity{0}Result {0}Result;
        ", method_info.Name, argTypes);
            return code;
        }
        private string _GetArgDefines(Type[] generic_arguments)
        {
            var types = new List<string>();
            for (int i = 0; i < generic_arguments.Length; i++)
            {
                var type = generic_arguments[i];
                types.Add(type.FullName + " arg" + i);
            }

            return string.Join(",", types.ToArray());
        }
        private static string _GetArgTypes(Type[] generic_arguments)
        {
            if (generic_arguments.Any())
                return "<" + string.Join(",", (from arg in generic_arguments select arg.ToString().Replace("+",".")).ToArray()) + ">";
            return String.Empty;
        }

        private string _GenerateEvents(Type type)
        {
            var codes = new List<string>();
            var eventInfos = type.GetEvents();
            foreach (var eventInfo in eventInfos)
            {
                codes.Add(_GenerateEventDefine(eventInfo));
                codes.Add(_GenerateEventFunction(eventInfo));
            }
            return string.Join("\n", codes.ToArray());
        }

        private string _GenerateEventFunction(EventInfo event_info)
        {

            var argTypes = event_info.EventHandlerType.GetGenericArguments();
            var argNames = _GetArgNames(argTypes);
            var argDefines = _GetArgDefines(argTypes);
            string code = String.Format(@"        
        private void _On{0}({1})
        {{
            {0}.Invoke({2});
        }}
        ", event_info.Name, argDefines, argNames);
            return code;
        }

        private string _GetArgNames(Type[] arg_types)
        {
            return string.Join(",", _GenArgNumber(arg_types.Length).ToArray());
        }

        private IEnumerable<string> _GenArgNumber(int length)
        {
            for (int i = 0; i < length; i++)
            {
                yield return "arg" + i;
            }
        }

        private string _GenerateEventDefine(EventInfo event_info)
        {
            var argTypes = _GetArgTypes(event_info.EventHandlerType.GetGenericArguments());

            string code = String.Format(@"
        [System.Serializable]
        public class Unity{0} : UnityEngine.Events.UnityEvent{1} {{ }}
        public Unity{0} {0};
        ", event_info.Name, argTypes);
            return code;
        }

        private string _GenerateMethods(Type type)
        {
            var methodInfos = type.GetMethods();
            var methodCodes = new List<string>();
            foreach (var methodInfo in methodInfos)
            {
                if (methodInfo.IsSpecialName)
                {
                    continue;
                }
                var returnType = methodInfo.ReturnType;

                if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Regulus.Remote.Value<>))
                {
                    methodCodes.Add(_GenerateMethodHaveReturn(type, methodInfo));
                }
                else
                {
                    methodCodes.Add(_GenerateMethodNotReturn(type, methodInfo));
                }
            }
            return string.Join("\n", methodCodes.ToArray());
        }

        private string _GenerateMethodNotReturn(Type type, MethodInfo method_info)
        {
            string code = string.Format(@"
        public void {0}({1})
        {{
            if(_{2} != null)
            {{
                _{2}.{0}({3});
            }}
        }}", method_info.Name, _GetParamsDefine(method_info), _GetClassName(type.Name), _GetParams(method_info));
            return code;
        }

        private string _GenerateMethodHaveReturn(Type type, MethodInfo method_info)
        {
            string code = string.Format(@"
        public void {0}({1})
        {{
            if(_{2} != null)
            {{
                _{2}.{0}({3}).OnValue += ( result ) =>{{ {0}Result.Invoke(result);}};
            }}
        }}", method_info.Name, _GetParamsDefine(method_info), _GetClassName(type.Name), _GetParams(method_info));

            return code;
        }

        private object _GetParams(MethodInfo method_info)
        {
            var args = new List<string>();
            foreach (var paramInfo in method_info.GetParameters())
            {
                args.Add(paramInfo.Name);
            }
            return String.Join(",", args.ToArray());
        }

        private string _GetParamsDefine(MethodInfo method_info)
        {
            var args = new List<string>();
            foreach (var paramInfo in method_info.GetParameters())
            {
                args.Add(paramInfo.ParameterType.ToString() + " " + paramInfo.Name);
            }
            return String.Join(",", args.ToArray());
        }

        private string _GetClassName(string type_name)
        {
            var className = new string(type_name.Skip(1).ToArray());
            return className;
        }
    }
}