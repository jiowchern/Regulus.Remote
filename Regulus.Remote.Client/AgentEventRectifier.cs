using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Remote.Client
{

    // jit
    public class AgentEventRectifier : IDisposable
    {
        readonly System.Collections.Generic.List<System.Action> _RemoveHandlers;
        private readonly Type[] _Types;

        public AgentEventRectifier(IEnumerable<Type> types, INotifierQueryable agent_instance)
        {
            this._Types = types.ToArray();
            _RemoveHandlers = new List<System.Action>();


            var agentType = typeof(INotifierQueryable);
            var agentQueryNotifier = agentType.GetMethod(nameof(INotifierQueryable.QueryNotifier));
            
            foreach (var type in _Types)
            {

                var agentQueryNotifierT = agentQueryNotifier.MakeGenericMethod(type);
                

                var notifyInstance = agentQueryNotifierT.Invoke(agent_instance, new object[0]);

                var notifyTypeT = typeof(INotifier<>).MakeGenericType(type);
                

                var notifySupply = notifyTypeT.GetEvent(nameof(INotifier<object>.Supply));
                var notifyUnsupply = notifyTypeT.GetEvent(nameof(INotifier<object>.Unsupply));
                
                var catcherSupply = new Regulus.Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<Action<AgentEventRectifier>>)(ins => ins._Supply<object>(null)));                
                var supplyGenericMethod = catcherSupply.Method.GetGenericMethodDefinition();
                var supplyMethod = supplyGenericMethod.MakeGenericMethod(type);

                var catcherUnsupply = new Regulus.Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<Action<AgentEventRectifier>>)(ins => ins._Unsupply<object>(null)));
                var unsupplyGenericMethod = catcherUnsupply.Method.GetGenericMethodDefinition();
                var unsupplyMethod = unsupplyGenericMethod.MakeGenericMethod(type);

                var actionT1 = typeof(System.Action<>);
                var actionT = actionT1.MakeGenericType(type);



                var delegateSupply = Delegate.CreateDelegate(actionT, this, supplyMethod);
                var delegateUnsupply = Delegate.CreateDelegate(actionT, this, unsupplyMethod);
                notifySupply.AddEventHandler(notifyInstance, delegateSupply);
                notifyUnsupply.AddEventHandler(notifyInstance, delegateUnsupply);
                _RemoveHandlers.Add(() => {
                    notifySupply.RemoveEventHandler(notifyInstance , delegateSupply);
                    notifyUnsupply.RemoveEventHandler(notifyInstance, delegateUnsupply);
                });
            }

            
        }

        
        private void _Supply<T>(T instance)
        {
            var type = instance.GetType();
            var parents = type.GetInterfaces();
            foreach(var p in parents)
            {
                if(_Types.Any( t => t== p))
                    SupplyEvent(p, instance);
            }
            
        }

        private void _Unsupply<T>(T instance)
        {

            var type = instance.GetType();
            var parents = type.GetInterfaces();
            foreach (var p in parents)
            {
                if (_Types.Any(t => t == p))
                    UnsupplyEvent(p, instance);
            }

            
        }

        void IDisposable.Dispose()
        {
            foreach (var removeHandler in _RemoveHandlers)
                removeHandler();
        }

        public event System.Action<Type, object> SupplyEvent;
        public event System.Action<Type, object> UnsupplyEvent;
    }
}
