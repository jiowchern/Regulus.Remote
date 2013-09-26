using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class Command
    {
        static void _Cnv(string p, out object val, Type source)
        {

            val = p;

            if (source == typeof(int))
            {
                int reault = int.MinValue;
                if (int.TryParse(p, out reault))
                {

                }
                val = reault;
            }
            else if (source == typeof(float))
            {
                float reault = float.MinValue;
                if (float.TryParse(p, out reault))
                {

                }
                val = reault;
            }



        }

        class Infomation
        {
            public string Name;
            public Action<string[]> Handler;            
        }
        List<Infomation> _Commands;

        public Command()
        {
            _Commands = new List<Infomation>();
            RegisterEvent += _EmptyRegisterEvent;
            UnregisterEvent += (s) => { };
        }

        void _EmptyRegisterEvent(string command, Type ret, Type[] args)
        {
            //throw new NotImplementedException();
        }
        private void _AddCommand(string command, Action<string[]> func)
        {
            _Commands.Add(new Infomation() { Name = command, Handler = func});
        }


        public delegate void OnRegister(string command, Type ret , Type[] args);
        public event OnRegister RegisterEvent;

        public delegate void OnUnregister(string command);
        public event OnUnregister UnregisterEvent;
        public void Register(string command, Action executer)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length == 0)
                {
                    executer.Invoke();
                }
            };

            _AddCommand(command, func);
            RegisterEvent(command , null , new Type[0] );
        }

        
        public void Register<T1>(string command, Action<T1> executer)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length == 1)
                {
                    object arg0;
                    _Cnv(args[0], out arg0, typeof(T1));
                    executer.Invoke((T1)arg0);
                }
            };

            _AddCommand(command, func);
            RegisterEvent(command, null, new Type[]{typeof(T1)});
        }

        public void Register<T1, T2>(string command, Action<T1, T2> executer)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length == 2)
                {
                    object arg0;
                    _Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    _Cnv(args[1], out arg1, typeof(T2));
                    executer.Invoke((T1)arg0, (T2)arg1);
                }
            };

            _AddCommand(command, func);
            RegisterEvent(command, null, new Type[] { typeof(T1), typeof(T2) });
        }
        public void Register<T1, T2, T3>(string command, Action<T1, T2, T3> executer)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length == 3)
                {
                    object arg0;
                    _Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    _Cnv(args[1], out arg1, typeof(T2));
                    object arg2;
                    _Cnv(args[2], out arg2, typeof(T3));
                    executer.Invoke((T1)arg0, (T2)arg1, (T3)arg2);
                }
            };

            _AddCommand(command, func);
            RegisterEvent(command, null, new Type[] { typeof(T1), typeof(T2), typeof(T3) });
        }
        public void Register<T1, T2, T3, T4>(string command, Action<T1, T2, T3, T4> executer)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length == 4)
                {
                    object arg0;
                    _Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    _Cnv(args[1], out arg1, typeof(T2));
                    object arg2;
                    _Cnv(args[2], out arg2, typeof(T3));
                    object arg3;
                    _Cnv(args[3], out arg3, typeof(T4));
                    executer.Invoke((T1)arg0, (T2)arg1, (T3)arg2, (T4)arg3);
                }
            };

            _AddCommand(command, func);
            RegisterEvent(command, null, new Type[] { typeof(T1), typeof(T2), typeof(T3) , typeof(T4)});
        }

        public void Register<TR>(string command, Func<TR> executer, Action<TR> value)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length == 0)
                {
                    var ret = executer.Invoke();
                    if (ret != null && value != null)
                    {
                        value((TR)ret);
                    }
                }
            };

            _AddCommand(command, func);
            RegisterEvent(command, typeof(TR) , new Type[0]);
        }

        public void Register<T1, TR>(string command, Func<T1, TR> executer, Action<TR> value)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length == 1)
                {
                    object arg0;
                    _Cnv(args[0], out arg0, typeof(T1));
                    var ret = executer.Invoke((T1)arg0);
                    if (ret != null && value != null)
                    {
                        value((TR)ret);
                    }
                }
            };

            _AddCommand(command, func);
            RegisterEvent(command, typeof(TR), new Type[] { typeof(T1) });
        }

        public void Register<T1, T2, TR>(string command, Func<T1, T2, TR> executer, Action<TR> value)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length == 2)
                {
                    object arg0;
                    _Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    _Cnv(args[1], out arg1, typeof(T2));

                    var ret = executer.Invoke((T1)arg0, (T2)arg1);
                    if (ret != null && value != null)
                    {
                        value((TR)ret);
                    }
                }
            };

            _AddCommand(command, func);
            RegisterEvent(command, typeof(TR), new Type[] { typeof(T1), typeof(T2) });
        }

        public void Register<T1, T2, T3, TR>(string command, Func<T1, T2, T3, TR> executer, Action<TR> value)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length == 3)
                {
                    object arg0;
                    _Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    _Cnv(args[1], out arg1, typeof(T2));
                    object arg2;
                    _Cnv(args[2], out arg2, typeof(T3));

                    var ret = executer.Invoke((T1)arg0, (T2)arg1, (T3)arg2);
                    if (ret != null && value != null)
                    {
                        value((TR)ret);
                    }
                }
            };

            _AddCommand(command, func);
            RegisterEvent(command, typeof(TR), new Type[] { typeof(T1), typeof(T2), typeof(T3) });
        }

        public void Register<T1, T2, T3, T4, TR>(string command, Func<T1, T2, T3, T4, TR> executer, Action<TR> value)
        {
            Action<string[]> func = (args) =>
            {
                if (args.Length == 3)
                {
                    object arg0;
                    _Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    _Cnv(args[1], out arg1, typeof(T2));
                    object arg2;
                    _Cnv(args[2], out arg2, typeof(T3));
                    object arg3;
                    _Cnv(args[3], out arg3, typeof(T4));

                    var ret = executer.Invoke((T1)arg0, (T2)arg1, (T3)arg2, (T4)arg3);
                    if (ret != null && value != null)
                    {
                        value((TR)ret);
                    }
                }
            };

            _AddCommand(command, func);
            RegisterEvent(command, typeof(TR), new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
        }

        public void Unregister(string command)
        {
            _Commands.RemoveAll(cmd => cmd.Name == command);
            UnregisterEvent(command);
        }


        public int Run(string command, string[] args)
        {
            var commandInfomations = (from ci in _Commands where ci.Name.ToLower() == command.ToLower() select ci);
            foreach(var commandInfomation in commandInfomations)
            {
                commandInfomation.Handler(args);
            }

            return commandInfomations.Count() ;
        }


    }
}
