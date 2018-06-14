using System;

namespace Regulus.Utility
{
    public abstract class Invoker : Notifier
    {
        
        protected new void Invoke()
        {
            base.Invoke();
        }
        
    }

    public abstract class Invoker<T> : Notifier<T>
    {
        protected new void Invoke(T info)
        {
            base.Invoke(info);
        }
    }


    public abstract class Invoker<T1,T2> : Notifier<Tuple<T1,T2>>
    {
        protected void Invoke(T1 arg1 , T2 arg2)
        {
            base.Invoke(new Tuple<T1, T2>(arg1,arg2));
        }
    }
}