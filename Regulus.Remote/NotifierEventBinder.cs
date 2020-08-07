using System;
using System.Reflection;

namespace Regulus.Remote
{
    class NotifierEventBinder
    {
        private readonly EventInfo _Info;
        readonly object _Instance;
        private readonly Delegate _Delegate;

        private readonly Action<object> _Invoker;

        public static NotifierEventBinder Create(object instance, PropertyInfo property, string event_name, Action<object> invoker)
        {
            Type notifierType = property.PropertyType;

            if (!notifierType.IsInterface)
            {
                return null;
            }
            if (notifierType.GetGenericTypeDefinition() != typeof(INotifier<>))
                return null;

            object notifier = property.GetValue(instance);
            return new NotifierEventBinder(notifier, notifierType.GetEvent(event_name), invoker);
        }
        public NotifierEventBinder(object notifier, EventInfo info, Action<object> invoker)
        {
            _Info = info;
            _Instance = notifier;
            _Invoker = invoker;
            Type type = info.DeclaringType.GetGenericArguments()[0];

            Utility.Reflection.TypeMethodCatcher catcherSupply = new Regulus.Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<Action<NotifierEventBinder>>)(ins => ins._Supply<object>(null)));
            MethodInfo supplyGenericMethod = catcherSupply.Method.GetGenericMethodDefinition();
            MethodInfo supplyMethod = supplyGenericMethod.MakeGenericMethod(type);

            Type actionT1 = typeof(System.Action<>);
            Type actionT = actionT1.MakeGenericType(type);
            Delegate delegateSupply = Delegate.CreateDelegate(actionT, this, supplyMethod);
            _Delegate = delegateSupply;

        }

        void _Supply<T>(T gpi)
        {
            _Invoker(gpi);
        }



        internal void Dispose()
        {
            _Info.RemoveEventHandler(_Instance, _Delegate);
        }

        internal void Setup()
        {

            _Info.AddEventHandler(_Instance, _Delegate);
        }
    }
}
