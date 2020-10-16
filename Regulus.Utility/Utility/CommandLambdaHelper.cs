using System;
using System.Linq.Expressions;

namespace Regulus.Utility
{
    /*
    public static class CommandLambdaHelper
    {
        public static void RegisterLambda<TThis, TR>(this Command this_obj, TThis instance, Expression<Func<TThis, TR>> exp, Action<TR> return_value)
        {
            Func<string[], object> func = (args) =>
            {
                if (args.Length != 0)
                {
                    throw new ArgumentException("命令參數數量為0");
                }

                return return_value(exp.Compile().Invoke(instance));
            };
            this_obj.Register(exp, func);
        }


        public static void RegisterLambda<TThis, T0, TR>(this Command this_obj, TThis instance, Expression<Func<TThis, T0, TR>> exp, Action<TR> return_value)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length != 1)
                {
                    throw new ArgumentException("命令參數數量為1");
                }

                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T0));

                return_value(exp.Compile().Invoke(instance, (T0)arg0));
            };
            this_obj.Register(exp, func);
        }

        public static void RegisterLambda<TThis, T0, T1, TR>(this Command this_obj, TThis instance, Expression<Func<TThis, T0, T1, TR>> exp, Action<TR> return_value)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length != 2)
                {
                    throw new ArgumentException("命令參數數量為2");
                }

                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T0));

                object arg1;
                Command.TryConversion(args[1], out arg1, typeof(T1));



                return_value(exp.Compile().Invoke(instance, (T0)arg0, (T1)arg1));
            };
            this_obj.Register(exp, func);
        }

        public static void RegisterLambda<TThis, T0, T1, T2, TR>(this Command this_obj, TThis instance, Expression<Func<TThis, T0, T1, T2, TR>> exp, Action<TR> return_value)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length != 3)
                {
                    throw new ArgumentException("命令參數數量為3");
                }

                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T0));

                object arg1;
                Command.TryConversion(args[1], out arg1, typeof(T1));

                object arg2;
                Command.TryConversion(args[2], out arg2, typeof(T2));

                return_value(exp.Compile().Invoke(instance, (T0)arg0, (T1)arg1, (T2)arg2));
            };
            this_obj.Register(exp, func);
        }

        public static void RegisterLambda<TThis, T0, T1, T2, T3, TR>(this Command this_obj, TThis instance, Expression<Func<TThis, T0, T1, T2, T3, TR>> exp, Action<TR> return_value)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length != 4)
                {
                    throw new ArgumentException("命令參數數量為4");
                }

                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T0));

                object arg1;
                Command.TryConversion(args[1], out arg1, typeof(T1));

                object arg2;
                Command.TryConversion(args[2], out arg2, typeof(T2));

                object arg3;
                Command.TryConversion(args[3], out arg3, typeof(T3));

                return_value(exp.Compile().Invoke(instance, (T0)arg0, (T1)arg1, (T2)arg2, (T3)arg3));
            };
            this_obj.Register(exp, func);
        }

        public static void RegisterLambda<TThis, T0, T1, T2, T3, T4, TR>(this Command this_obj, TThis instance, Expression<Func<TThis, T0, T1, T2, T3, T4, TR>> exp, Action<TR> return_value)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length != 5)
                {
                    throw new ArgumentException("命令參數數量為5");
                }

                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T0));

                object arg1;
                Command.TryConversion(args[1], out arg1, typeof(T1));

                object arg2;
                Command.TryConversion(args[2], out arg2, typeof(T2));

                object arg3;
                Command.TryConversion(args[3], out arg3, typeof(T3));

                object arg4;
                Command.TryConversion(args[4], out arg4, typeof(T4));

                return_value(exp.Compile().Invoke(instance, (T0)arg0, (T1)arg1, (T2)arg2, (T3)arg3, (T4)arg4));
            };
            this_obj.Register(exp, func);
        }


        public static void RegisterLambda<TThis>(this Command this_obj, TThis instance, Expression<Action<TThis>> exp)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length != 0)
                {
                    throw new ArgumentException("命令參數數量為0");
                }


                exp.Compile().Invoke(instance);
            };
            this_obj.Register(exp, func);
        }

        public static void RegisterLambda<TThis, T0>(this Command this_obj, TThis instance, Expression<Action<TThis, T0>> exp)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length != 1)
                {
                    throw new ArgumentException("命令參數數量為1");
                }
                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T0));

                exp.Compile().Invoke(instance, (T0)arg0);
            };
            this_obj.Register(exp, func);
        }

        public static void RegisterLambda<TThis, T0, T1>(this Command this_obj, TThis instance, Expression<Action<TThis, T0, T1>> exp)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length != 2)
                {
                    throw new ArgumentException("命令參數數量為2");
                }
                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T0));

                object arg1;
                Command.TryConversion(args[1], out arg1, typeof(T1));

                exp.Compile().Invoke(instance, (T0)arg0, (T1)arg1);
            };
            this_obj.Register(exp, func);
        }

        public static void RegisterLambda<TThis, T0, T1, T2>(this Command this_obj, TThis instance, Expression<Action<TThis, T0, T1, T2>> exp)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length != 3)
                {
                    throw new ArgumentException("命令參數數量為3");
                }
                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T0));

                object arg1;
                Command.TryConversion(args[1], out arg1, typeof(T1));

                object arg2;
                Command.TryConversion(args[2], out arg2, typeof(T2));

                exp.Compile().Invoke(instance, (T0)arg0, (T1)arg1, (T2)arg2);
            };
            this_obj.Register(exp, func);
        }

        public static void RegisterLambda<TThis, T0, T1, T2, T3>(this Command this_obj, TThis instance, Expression<Action<TThis, T0, T1, T2, T3>> exp)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length != 4)
                {
                    throw new ArgumentException("命令參數數量為4");
                }
                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T0));

                object arg1;
                Command.TryConversion(args[1], out arg1, typeof(T1));

                object arg2;
                Command.TryConversion(args[2], out arg2, typeof(T2));

                object arg3;
                Command.TryConversion(args[3], out arg3, typeof(T3));

                exp.Compile().Invoke(instance, (T0)arg0, (T1)arg1, (T2)arg2, (T3)arg3);
            };
            this_obj.Register(exp, func);
        }

        public static void RegisterLambda<TThis, T0, T1, T2, T3, T4>(this Command this_obj, TThis instance, Expression<Action<TThis, T0, T1, T2, T3, T4>> exp)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length != 5)
                {
                    throw new ArgumentException("命令參數數量為5");
                }
                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T0));

                object arg1;
                Command.TryConversion(args[1], out arg1, typeof(T1));

                object arg2;
                Command.TryConversion(args[2], out arg2, typeof(T2));

                object arg3;
                Command.TryConversion(args[3], out arg3, typeof(T3));

                object arg4;
                Command.TryConversion(args[4], out arg4, typeof(T4));

                exp.Compile().Invoke(instance, (T0)arg0, (T1)arg1, (T2)arg2, (T3)arg3, (T4)arg4);
            };
            this_obj.Register(exp, func);
        }
    }*/
}
