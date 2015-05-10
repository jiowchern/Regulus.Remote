using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{
    public class CommandParamBuilder
    {

        public CommandParam Build(Action callback)
        {
            return new CommandParam() { Callback = callback, Return = null, ReturnType = null, Types = new Type[0] };
        }
        public CommandParam Build<T1>(Action<T1> callback)
        {
            return new CommandParam() { Callback = callback, Types = new Type[] { typeof(T1) } , Return = null, ReturnType = null };
        }
        public CommandParam Build<T1, T2>(Action<T1, T2> callback)
        {
            return new CommandParam() { Callback = callback, Types = new Type[] { typeof(T1), typeof(T2) }, Return = null, ReturnType = null };
        }

        public CommandParam Build<T1, T2, T3>(Action<T1, T2, T3> callback)
        {
            return new CommandParam() { Callback = callback, Types = new Type[] { typeof(T1), typeof(T2), typeof(T3)}, Return = null, ReturnType = null };
        }

        public CommandParam Build<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback)
        {
            return new CommandParam() { Callback = callback, Types = new Type[] { typeof(T1), typeof(T2) , typeof(T3) ,  typeof(T4)  }, Return = null, ReturnType = null };
        }

        public CommandParam Build<TR>(Func<TR> callback, Action<TR> return_callback)
        {
            return new CommandParam() { Callback = callback, Types = new Type[] {}, Return = return_callback, ReturnType = typeof(TR) };
        }

        public CommandParam Build<T1, TR>(Func<T1, TR> callback, Action<TR> return_callback)
        {
            return new CommandParam() { Callback = callback, Types = new Type[] { typeof(T1)}, Return = return_callback, ReturnType = typeof(TR) };
        }

        public CommandParam Build<T1, T2, TR>(Func<T1, T2, TR> callback, Action<TR> return_callback)
        {
            return new CommandParam() { Callback = callback, Types = new Type[] { typeof(T1), typeof(T2) }, Return = return_callback, ReturnType = typeof(TR) };
        }

        public CommandParam Build<T1, T2, T3, TR>(Func<T1, T2, T3, TR> callback, Action<TR> return_callback)
        {
            return new CommandParam() { Callback = callback, Types = new Type[] { typeof(T1), typeof(T2), typeof(T3) }, Return = return_callback, ReturnType = typeof(TR) };
        }

        public CommandParam Build<T1, T2, T3, T4, TR>(Func<T1, T2, T3, T4, TR> callback, Action<TR> return_callback)
        {
            return new CommandParam() { Callback = callback, Types = new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, Return = return_callback, ReturnType = typeof(TR) };
        }

        public CommandParam BuildRemoting<TR>(Func<Regulus.Remoting.Value<TR>> callback, Action<TR> return_callback)
        {
            return new CommandParam()
            {
                Callback = callback,
                Types = new Type[] {},
                Return = new Action<Regulus.Remoting.Value<TR>>(
                    (tr) =>
                    {
                        tr.OnValue += return_callback;
                    }),
                ReturnType = typeof(Regulus.Remoting.Value<TR>)
            };
        }

        public CommandParam BuildRemoting<T1, TR>(Func<T1, Regulus.Remoting.Value<TR>> callback, Action<TR> return_callback)
        {
            return new CommandParam()
            {
                Callback = callback,
                Types = new Type[] { typeof(T1) },
                Return = new Action<Regulus.Remoting.Value<TR>>(
                    (tr) =>
                    {
                        tr.OnValue += return_callback;
                    }),
                ReturnType = typeof(Regulus.Remoting.Value<TR>)
            };
        }

        public CommandParam BuildRemoting<T1, T2, TR>(Func<T1, T2, Regulus.Remoting.Value<TR>> callback, Action<TR> return_callback)
        {
            return new CommandParam()
            {
                Callback = callback,
                Types = new Type[] { typeof(T1), typeof(T2) },
                Return = new Action<Regulus.Remoting.Value<TR>>(
                    (tr) =>
                    {
                        tr.OnValue += return_callback;
                    }),
                ReturnType = typeof(Regulus.Remoting.Value<TR>)
            };
        }

        public CommandParam BuildRemoting<T1, T2, T3, TR>(Func<T1, T2, T3, Regulus.Remoting.Value<TR>> callback, Action<TR> return_callback)
        {
            return new CommandParam()
            {
                Callback = callback,
                Types = new Type[] { typeof(T1), typeof(T2), typeof(T3) },
                Return = new Action<Regulus.Remoting.Value<TR>>(
                    (tr) =>
                    {
                        tr.OnValue += return_callback;
                    }),
                ReturnType = typeof(Regulus.Remoting.Value<TR>)
            };
        }

        public CommandParam BuildRemoting<T1, T2, T3, T4, TR>(Func<T1, T2, T3, T4, Regulus.Remoting.Value<TR>> callback, Action<TR> return_callback)
        {
            return new CommandParam()
            {
                Callback = callback,
                Types = new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) },
                Return = new Action<Regulus.Remoting.Value<TR>>(
                    (tr) => 
                    {
                        tr.OnValue += return_callback;                        
                    }),
                ReturnType = typeof(Regulus.Remoting.Value<TR>)
            };
        }
    }
}
