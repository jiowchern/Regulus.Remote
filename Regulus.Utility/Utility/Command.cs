using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;

namespace Regulus.Utility
{
    public delegate void Action<in T1, in T2, in T3, in T4, in T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

    //public delegate void Func<in T1, in T2, in T3, in T4, in T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, out TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    public partial class Command : ICommand
    {
        public delegate void OnRegister(string command, CommandParameter ret, CommandParameter[] args);

        public delegate void OnUnregister(string command);

        public event OnRegister RegisterEvent;

        public event OnUnregister UnregisterEvent;

        private readonly List<Infomation> _Commands;

        public Command()
        {
            _Commands = new List<Infomation>();
            RegisterEvent += _EmptyRegisterEvent;
            UnregisterEvent += s => { };
        }


        public System.Guid Register(string command, Action<string[]> func, Type return_type, Type[] param_types)
        {
            return _Register(command, func, return_type, param_types);
        }
        private System.Guid _Register(string command, Action<string[]> func, Type return_type, Type[] param_types)
        {
            Analysis analysis = new Analysis(command);
            Guid id = _AddCommand(analysis.Command, func);
            _SendRegister(analysis, return_type, param_types);
            return id;
        }
        public void Register(string command, Action executer)
        {
            Action<string[]> func = args =>
            {
                if (args.Length != 0)
                {
                    throw new ArgumentException("The number of command arguments is 0");
                }

                executer.Method.Invoke(executer.Target, new object[0]);
            };

            _Register(command, func, typeof(void), new Type[0]);
        }



        public void Register<T1>(string command, Action<T1> executer)
        {
            Action<string[]> func = args =>
            {
                if (args.Length != 1)
                {
                    throw new ArgumentException("The number of command arguments is 1");
                }

                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T1));
                T1 val = (T1)arg0;



                executer.Method.Invoke(executer.Target, new[] { arg0 });
            };


            _Register(command, func, typeof(void), new[]
                {
                    typeof(T1)
                });

        }

        public void Register<T1, T2>(string command, Action<T1, T2> executer)
        {
            Action<string[]> func = args =>
            {
                if (args.Length != 2)
                {
                    throw new ArgumentException("The number of command arguments is 2");
                }

                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T1));
                object arg1;
                Command.TryConversion(args[1], out arg1, typeof(T2));


                executer.Method.Invoke(executer.Target, new[] { arg0, arg1 });

            };

            _Register(command, func, typeof(void), new[]
                {
                    typeof(T1),
                    typeof(T2)
                });
        }

        public void Register<T1, T2, T3>(string command, Action<T1, T2, T3> executer)
        {
            Action<string[]> func = args =>
            {
                if (args.Length != 3)
                {
                    throw new ArgumentException("The number of command arguments is 3");
                }

                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T1));
                object arg1;
                Command.TryConversion(args[1], out arg1, typeof(T2));
                object arg2;
                Command.TryConversion(args[2], out arg2, typeof(T3));
                executer.Method.Invoke(executer.Target, new[] { arg0, arg1, arg2 });
            };


            _Register(command, func, typeof(void), new[]
                {
                    typeof(T1),
                    typeof(T2),
                    typeof(T3)
                });
        }

        public void Register<T1, T2, T3, T4>(string command, Action<T1, T2, T3, T4> executer)
        {
            Action<string[]> func = args =>
            {
                if (args.Length != 4)
                {
                    throw new ArgumentException("The number of command arguments is 4");
                }

                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T1));
                object arg1;
                Command.TryConversion(args[1], out arg1, typeof(T2));
                object arg2;
                Command.TryConversion(args[2], out arg2, typeof(T3));
                object arg3;
                Command.TryConversion(args[3], out arg3, typeof(T4));
                executer.Method.Invoke(executer.Target, new[] { arg0, arg1, arg2, arg3 });
            };

            _Register(command, func, typeof(void), new[]
                {
                    typeof(T1),
                    typeof(T2),
                    typeof(T3),
                    typeof(T4)
                });
        }

        public void Register<TR>(string command, Func<TR> executer, Action<TR> value)
        {
            Action<string[]> func = args =>
            {
                if (args.Length != 0)
                {
                    throw new ArgumentException("The number of command arguments is 0");
                }

                object ret = executer.Method.Invoke(executer.Target, new object[0]);
                value.Method.Invoke(value.Target, new object[] { ret });
            };

            _Register(command, func, typeof(TR), new Type[0]);

        }

        public void Register<T1, TR>(string command, Func<T1, TR> executer, Action<TR> value)
        {
            Action<string[]> func = args =>
            {
                if (args.Length != 1)
                {
                    throw new ArgumentException("The number of command arguments is 1");
                }

                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T1));
                object ret = executer.Method.Invoke(executer.Target, new object[] { arg0 });
                value.Method.Invoke(value.Target, new object[] { ret });


            };

            _Register(command, func, typeof(TR), new[]
                {
                    typeof(T1)
                });


        }

        public void Register<T1, T2, TR>(string command, Func<T1, T2, TR> executer, Action<TR> value)
        {
            Action<string[]> func = args =>
            {
                if (args.Length != 2)
                {
                    throw new ArgumentException("The number of command arguments is 2");
                }

                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T1));
                object arg1;
                Command.TryConversion(args[1], out arg1, typeof(T2));

                object ret = executer.Method.Invoke(executer.Target, new object[] { arg0, arg1 });
                value.Method.Invoke(value.Target, new object[] { ret });
            };

            _Register(command, func, typeof(TR), new[]
                {
                    typeof(T1),
                    typeof(T2)
                });


        }

        public void Register<T1, T2, T3, TR>(string command, Func<T1, T2, T3, TR> executer, Action<TR> value)
        {
            Action<string[]> func = args =>
            {
                if (args.Length != 3)
                {
                    throw new ArgumentException("The number of command arguments is 3");
                }

                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T1));
                object arg1;
                Command.TryConversion(args[1], out arg1, typeof(T2));
                object arg2;
                Command.TryConversion(args[2], out arg2, typeof(T3));

                object ret = executer.Method.Invoke(executer.Target, new[] { arg0, arg1, arg2 });
                value.Method.Invoke(value.Target, new[] { ret });
            };

            _Register(command, func, typeof(TR), new[]
                {
                    typeof(T1),
                    typeof(T2),
                    typeof(T3)
                });

        }

        public void Register<T1, T2, T3, T4, TR>(string command, Func<T1, T2, T3, T4, TR> executer, Action<TR> value)
        {
            Action<string[]> func = args =>
            {
                if (args.Length != 4)
                {
                    throw new ArgumentException("The number of command arguments is 4");
                }

                object arg0;
                Command.TryConversion(args[0], out arg0, typeof(T1));
                object arg1;
                Command.TryConversion(args[1], out arg1, typeof(T2));
                object arg2;
                Command.TryConversion(args[2], out arg2, typeof(T3));
                object arg3;
                Command.TryConversion(args[3], out arg3, typeof(T4));

                object ret = executer.Method.Invoke(executer.Target, new[] { arg0, arg1, arg2, arg3 });
                value.Method.Invoke(value.Target, new[] { ret });
            };

            _Register(command, func, typeof(TR), new[]
                {
                    typeof(T1),
                    typeof(T2),
                    typeof(T3),
                    typeof(T4)
                });
        }

        public void Unregister(string command)
        {
            Analysis analysis = new Analysis(command);
            if (_Commands.RemoveAll(cmd => cmd.Name == analysis.Command) > 0)
            {
                UnregisterEvent(analysis.Command);
            }
        }

        public void Unregister(System.Guid id)
        {
            Infomation cmd = _Commands.FirstOrDefault(info => info.Id == id);
            if (cmd != null && _Commands.RemoveAll(info => info.Id == id) > 0)
            {
                UnregisterEvent(cmd.Name);
            }
        }
        static object _GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
        public static bool TryConversion(string in_string, out object out_value, Type source)
        {
            out_value = _GetDefault(source);

            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(source);
                out_value = converter.ConvertFromString(in_string);
                return true;
            }
            catch (Exception ex)
            {
                if (source == typeof(IPEndPoint))
                {
                    try
                    {
                        Match m = Regex.Match(in_string, @"(\d+\.\d+\.\d+\.\d+):(\d+)");
                        if (m.Success)
                        {
                            string address = m.Groups[1].Value;
                            string port = m.Groups[2].Value;
                            out_value = new IPEndPoint(IPAddress.Parse(address), int.Parse(port));
                        }
                        else
                        {
                            out_value = new IPEndPoint(0, 0);
                        }

                    }
                    catch (SystemException se)
                    {
                        out_value = new IPEndPoint(0, 0);
                    }


                }
                else
                {

                    Regulus.Utility.Log.Instance.WriteInfo($"Type conversion failed. Type:{source} Value:[{in_string}]");
                }


            }

            return false;

        }

        private void _EmptyRegisterEvent(string command, CommandParameter ret, CommandParameter[] args)
        {
            // throw new NotImplementedException();
        }

        private System.Guid _AddCommand(string command, Action<string[]> func)
        {
            Infomation info = new Infomation(command, func);
            _Commands.Add(info);

            return info.Id;
        }

        public int Run(string command, string[] args)
        {
            IEnumerable<Infomation> commandInfomations = from ci in _Commands where ci.Name.ToLower() == command.ToLower() select ci;
            List<Infomation> infos = new List<Infomation>();

            foreach (Infomation commandInfomation in commandInfomations)
            {
                infos.Add(commandInfomation);
            }

            foreach (Infomation info in infos)
            {
                info.Handler(args);
            }

            return infos.Count();
        }

        private void _SendRegister(Analysis analysis, Type ret, Type[] args)
        {
            Type[] parameterTypes = args.ToArray();
            string[] parameterDescs = analysis.Parameters.ToArray();
            RegisterEvent(
                analysis.Command,
                new CommandParameter(ret, analysis.Return),
                _BuildCommandParameters(parameterTypes, parameterDescs));
        }

        private CommandParameter[] _BuildCommandParameters(Type[] parameterTypes, string[] parameterDescs)
        {
            int count = parameterTypes.Length;
            CommandParameter[] cps = new CommandParameter[count];
            for (int i = 0; i < count; ++i)
            {
                cps[i] = new CommandParameter(
                    parameterTypes[i],
                    (i < parameterDescs.Length)
                        ? parameterDescs[i]
                        : string.Empty);
            }

            return cps;
        }

        public void Register(LambdaExpression exp, Action<string[]> func)
        {
            _Register(exp, func);
        }
        private void _Register(LambdaExpression exp, Action<string[]> func)
        {

            if (exp.Body.NodeType != ExpressionType.Call)
            {
                throw new ArgumentException();
            }

            MethodCallExpression methodCall = exp.Body as MethodCallExpression;
            System.Reflection.MethodInfo method = methodCall.Method;

            string[] argNames = (from par in exp.Parameters.Skip(1) select par.Name).ToArray();
            Type[] argTypes = (from par in exp.Parameters.Skip(1) select par.Type).ToArray();

            string commandString = string.Format("{0} [{1}  ] [{2} ]", method.Name, string.Join(",", argNames.ToArray()), "return_" + method.ReturnType.Name);
            _Register(commandString, func, method.ReturnType, argTypes.ToArray());
        }
    }
}
