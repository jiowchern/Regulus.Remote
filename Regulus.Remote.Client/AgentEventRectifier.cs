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


            Type agentType = typeof(INotifierQueryable);
            System.Reflection.MethodInfo agentQueryNotifier = agentType.GetMethod(nameof(INotifierQueryable.QueryNotifier));

            foreach (Type type in _Types)
            {

                System.Reflection.MethodInfo agentQueryNotifierT = agentQueryNotifier.MakeGenericMethod(type);


                object notifyInstance = agentQueryNotifierT.Invoke(agent_instance, new object[0]);

                Type notifyTypeT = typeof(INotifier<>).MakeGenericType(type);


                System.Reflection.EventInfo notifySupply = notifyTypeT.GetEvent(nameof(INotifier<object>.Supply));
                System.Reflection.EventInfo notifyUnsupply = notifyTypeT.GetEvent(nameof(INotifier<object>.Unsupply));

                Utility.Reflection.TypeMethodCatcher catcherSupply = new Regulus.Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<Action<AgentEventRectifier>>)(ins => ins._Supply<object>(null)));
                System.Reflection.MethodInfo supplyGenericMethod = catcherSupply.Method.GetGenericMethodDefinition();
                System.Reflection.MethodInfo supplyMethod = supplyGenericMethod.MakeGenericMethod(type);

                Utility.Reflection.TypeMethodCatcher catcherUnsupply = new Regulus.Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<Action<AgentEventRectifier>>)(ins => ins._Unsupply<object>(null)));
                System.Reflection.MethodInfo unsupplyGenericMethod = catcherUnsupply.Method.GetGenericMethodDefinition();
                System.Reflection.MethodInfo unsupplyMethod = unsupplyGenericMethod.MakeGenericMethod(type);

                Type actionT1 = typeof(System.Action<>);
                Type actionT = actionT1.MakeGenericType(type);



                Delegate delegateSupply = Delegate.CreateDelegate(actionT, this, supplyMethod);
                Delegate delegateUnsupply = Delegate.CreateDelegate(actionT, this, unsupplyMethod);
                notifySupply.AddEventHandler(notifyInstance, delegateSupply);
                notifyUnsupply.AddEventHandler(notifyInstance, delegateUnsupply);
                _RemoveHandlers.Add(() =>
                {
                    notifySupply.RemoveEventHandler(notifyInstance, delegateSupply);
                    notifyUnsupply.RemoveEventHandler(notifyInstance, delegateUnsupply);
                });
            }


        }


        private void _Supply<T>(T instance)
        {
            Type type = instance.GetType();
            Type[] parents = type.GetInterfaces();
            foreach (Type p in parents)
            {
                if (_Types.Any(t => t == p))
                    SupplyEvent(p, instance);
            }

        }

        private void _Unsupply<T>(T instance)
        {

            Type type = instance.GetType();
            Type[] parents = type.GetInterfaces();
            foreach (Type p in parents)
            {
                if (_Types.Any(t => t == p))
                    UnsupplyEvent(p, instance);
            }


        }

        void IDisposable.Dispose()
        {
            foreach (Action removeHandler in _RemoveHandlers)
                removeHandler();
        }

        public event System.Action<Type, object> SupplyEvent;
        public event System.Action<Type, object> UnsupplyEvent;
    }
}
