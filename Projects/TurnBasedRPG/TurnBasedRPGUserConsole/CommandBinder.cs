using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.TurnBasedRPGUserConsole
{
    using Regulus.Project.TurnBasedRPG.Common;
    using Regulus.Project.TurnBasedRPG.Serializable;
    using Samebest.Remoting;    
    class CommandBinder
    {
        CommandHandler _CommandHandler;
        TurnBasedRPG.User _User;
        public CommandBinder(CommandHandler command_handler, TurnBasedRPG.User user)
        {
            _CommandHandler = command_handler;
            _User = user; 
        }
        internal void Setup()
        {
            var fw = _User as Samebest.Game.IFramework;
            _CommandHandler.Set("quit", _Build(fw.Shutdown), "離開 ex. quit");
            _Bind(_User.Complex.QueryProvider<IVerify>());
            _Bind(_User.Complex.QueryProvider<IParking>());
            _Bind(_User.Complex.QueryProvider<IPlayer>());
        }

        private void _Bind(Samebest.Remoting.Ghost.IProviderNotice<IPlayer> providerNotice)
        {
            providerNotice.Supply += _PlayerSupply;
            providerNotice.Unsupply += _PlayerUnsupply;
        }

        private void _PlayerUnsupply(IPlayer obj)
        {
            _CommandHandler.Rise("ExitWorld");
            _CommandHandler.Rise("Logout");
            _CommandHandler.Rise("SetData");
        }

        private void _PlayerSupply(IPlayer obj)
        {
            _CommandHandler.Set("ExitWorld", _Build(obj.ExitWorld), "返回選角 ex. ExitWorld");
            _CommandHandler.Set("Logout", _Build(obj.Logout), "離開遊戲 ex. Logout");

            Action<Value<int>> setDataResult = (value)=>
            {
                value.OnValue += (res) => 
                {
                    Console.WriteLine("取得資料"+ res.ToString());
                };
            };
            _CommandHandler.Set("SetData", _Build<int, int>(obj.SetData, setDataResult), "測試用 SetData [數字]");
        }

        private void _Bind(Samebest.Remoting.Ghost.IProviderNotice<IParking> providerNotice)
        {
            providerNotice.Supply += _ParkingSupply;
            providerNotice.Unsupply += _ParkingUnupply;
        }

        private void _ParkingUnupply(IParking obj)
        {
            _CommandHandler.Rise("CheckActorName");
            _CommandHandler.Rise("CreateActor");
            _CommandHandler.Rise("DestroyActor");
            _CommandHandler.Rise("Back");
            _CommandHandler.Rise("QueryActors");
            _CommandHandler.Rise("Select");
        }

        private void _ParkingSupply(IParking obj)
        {
            Action<Value<bool>> checkActorNameResult = (value)=>
            {
                value.OnValue += (res) => 
                {
                    if (res)
                    {
                        Console.WriteLine("角色名稱有重複.");
                    }
                    else
                        Console.WriteLine("角色名稱沒有重複.");
                };
            };
            _CommandHandler.Set("CheckActorName", _Build<string, bool>(obj.CheckActorName, checkActorNameResult), "檢查重複名稱 ex. CheckActorName [名稱]");

            Func<string , Value<bool>> createActor = (name)=>
            {
                var ai = new ActorInfomation();
                ai.Name = name;
                return obj.CreateActor(ai);
            };

            Action<Value<bool>> createActorResult = (value) =>
            {
                value.OnValue += (res) =>
                {
                    if (res)
                    {
                        Console.WriteLine("角色建立成功.");
                    }
                    else
                        Console.WriteLine("角色建立失敗.");
                };
            };

            _CommandHandler.Set("CreateActor", _Build<string, bool>(createActor, createActorResult), "建立新角色 ex. CreateActor [名稱]");


            Action<Value<ActorInfomation[]>> destroyActorResult = (value) =>
            {
                value.OnValue += (res) =>
                {
                    _ShowActors(res);
                };
            };
            _CommandHandler.Set("DestroyActor", _Build<string, ActorInfomation[]>(obj.DestroyActor, destroyActorResult), "刪除角色 ex. DestroyActor [名稱]");


            _CommandHandler.Set("Back", _Build(obj.Back), "返回登入 ex. Back ");


            Action<Value<ActorInfomation[]>> queryActorsResult = (value) =>
            {
                value.OnValue += (res) =>
                {
                    _ShowActors(res);
                };
            };
            _CommandHandler.Set("QueryActors", _Build<ActorInfomation[]>(obj.QueryActors, queryActorsResult), "查詢角色 ex. QueryActors");

            _CommandHandler.Set("Select", _Build<string>(obj.Select), "選擇角色 ex. select [名稱]");
        }

        private void _ShowActors(ActorInfomation[] ais)
        {
            foreach (var ai in ais)
            {
                Console.WriteLine(ai.Name);
            }
        }
        private void _Bind(Samebest.Remoting.Ghost.IProviderNotice<IVerify> providerNotice)
        {
            providerNotice.Supply += _VeriftSupply;
            providerNotice.Unsupply += _VeriftUnsupply;
        }

        private void _VeriftUnsupply(IVerify obj)
        {
            _CommandHandler.Rise("CreateAccount");
            _CommandHandler.Rise("Login");
        }

        private void _VeriftSupply(IVerify obj)
        {
            _CommandHandler.Set("CreateAccount", _Build<string, string , bool>(obj.CreateAccount, _CreateAccountResult), "建立帳號 ex. createaccount [帳號] [密碼]");
            _CommandHandler.Set("Login", _Build<string, string, bool>(obj.Login, _LoginAccountResult), "登入 ex. login [帳號] [密碼]");


            obj.RepeatLogin += () => { Console.WriteLine("重複登入");  };
        }

        private void _LoginAccountResult(Value<bool> obj)
        {
            obj.OnValue += (res) =>
            {
                if (res)
                    Console.WriteLine("登入成功.");
                else
                    Console.WriteLine("登入失敗.");
            };
        }

        private void _CreateAccountResult(Value<bool> obj)
        {
            obj.OnValue += (res) => 
            {
                if (res)
                    Console.WriteLine("帳號建立成功.");
                else
                    Console.WriteLine("帳號建立失敗.");
            };
        }


        private Action<string[]> _Build(Action func)
        {
            return (args) =>
            {
                if (args.Length == 0)
                {                    
                    func.Invoke();
                }
            };
        }

        private Action<string[]> _Build<T1>(Action<T1> func)
        {
            return (args) =>
            {
                if (args.Length == 1)
                {
                    object arg0;
                    ClosuresHelper.Cnv(args[0], out arg0 , typeof(T1));
                    func.Invoke((T1)arg0);
                }
            };
        }


        private Action<string[]> _Build<T1, T2>(Action<T1, T2> func)
        {
            return (args) =>
            {
                if (args.Length == 2)
                {
                    object arg0;
                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    ClosuresHelper.Cnv(args[1], out arg1, typeof(T2));

                    func.Invoke((T1)arg0, (T2)arg1);
                }
            };
        }


        private Action<string[]> _Build<T1, T2, T3>(Action<T1, T2, T3> func)
        {
            return (args) =>
            {
                if (args.Length == 3)
                {
                    object arg0;
                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    ClosuresHelper.Cnv(args[1], out arg1, typeof(T2));
                    object arg2;
                    ClosuresHelper.Cnv(args[2], out arg2, typeof(T3));
                    

                    func.Invoke((T1)arg0, (T2)arg1, (T3)arg2 );
                }
            };
        }

        private Action<string[]> _Build<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func)
        {
            return (args) =>
            {
                if (args.Length == 4)
                {
                    object arg0;
                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    ClosuresHelper.Cnv(args[1], out arg1, typeof(T2));
                    object arg2;
                    ClosuresHelper.Cnv(args[2], out arg2, typeof(T3));
                    object arg3;
                    ClosuresHelper.Cnv(args[3], out arg3, typeof(T4));
                    
                    func.Invoke((T1)arg0, (T2)arg1, (T3)arg2, (T4)arg3 );
                }
            };
        }

        delegate void Action5<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        private Action<string[]> _Build<T1, T2, T3, T4, T5>(Action5<T1, T2, T3, T4, T5> func)
        {
            return (args) =>
            {
                if (args.Length == 5)
                {
                    object arg0;
                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    ClosuresHelper.Cnv(args[1], out arg1, typeof(T2));
                    object arg2;
                    ClosuresHelper.Cnv(args[2], out arg2, typeof(T3));
                    object arg3;
                    ClosuresHelper.Cnv(args[3], out arg3, typeof(T4));
                    object arg4;
                    ClosuresHelper.Cnv(args[4], out arg4, typeof(T5));
                    func.Invoke((T1)arg0, (T2)arg1, (T3)arg2, (T4)arg3, (T5)arg4);                    
                }
            };
        }

        private Action<string[]> _Build<TR>(Func<Value<TR>> func, Action<Value<TR>> value)
        {
            return (args) =>
            {
                if (args.Length == 0)
                {                    
                    var ret = func.Invoke();
                    if (ret != null && value != null)
                    {
                        value((Value<TR>)ret);
                    }
                }
            };
        }

        private Action<string[]> _Build<T1, TR>(Func<T1, Value<TR>> func, Action<Value<TR>> value)
        {
            return (args) =>
            {
                if (args.Length == 1)
                {
                    object arg0;

                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));                    
                    var ret = func.Invoke((T1)arg0);
                    if (ret != null && value != null)
                    {
                        value((Value<TR>)ret);
                    }
                }
            };
        }

        private Action<string[]> _Build<T1, T2, TR>(Func<T1, T2, Value<TR>> func, Action<Value<TR>> value) 
        {
            return (args) =>
            {
                if (args.Length == 2)
                {
                    object arg0;
                    object arg1;
                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));
                    ClosuresHelper.Cnv(args[1], out arg1, typeof(T2));
                    var ret = func.Invoke((T1)arg0, (T2)arg1);
                    if (ret != null && value != null)
                    {
                        value( (Value <TR>) ret);
                    }
                }
            };
        }

        private Action<string[]> _Build<T1, T2, T3, TR>(Func<T1, T2, T3, Value<TR>> func, Action<Value<TR>> value)
        {
            return (args) =>
            {
                if (args.Length == 3)
                {
                    object arg0;
                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    ClosuresHelper.Cnv(args[1], out arg1, typeof(T2));
                    object arg2;
                    ClosuresHelper.Cnv(args[2], out arg2, typeof(T3));                    

                    var ret = func.Invoke((T1)arg0, (T2)arg1, (T3)arg2 );
                    if (ret != null && value != null)
                    {
                        value((Value<TR>)ret);
                    }
                }
            };
        }

        private Action<string[]> _Build<T1, T2, T3, T4, TR>(Func<T1, T2, T3, T4, Value<TR>> func, Action<Value<TR>> value)
        {
            return (args) =>
            {
                if (args.Length == 4)
                {
                    object arg0;
                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    ClosuresHelper.Cnv(args[1], out arg1, typeof(T2));
                    object arg2;
                    ClosuresHelper.Cnv(args[2], out arg2, typeof(T3));
                    object arg3;
                    ClosuresHelper.Cnv(args[3], out arg3, typeof(T4));
                    
                    var ret = func.Invoke((T1)arg0, (T2)arg1, (T3)arg2, (T4)arg3);
                    if (ret != null && value != null)
                    {
                        value((Value<TR>)ret);
                    }
                }
            };
        }
        delegate TR Func5<T1, T2, T3, T4, T5,TR>(T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4);
        private Action<string[]> _Build<T1, T2, T3, T4, T5, TR>(Func5<T1, T2, T3, T4, T5, Value<TR>> func, Action<Value<TR>> value)
        {
            return (args) =>
            {
                if (args.Length == 5)
                {
                    object arg0;
                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    ClosuresHelper.Cnv(args[1], out arg1, typeof(T2));
                    object arg2;
                    ClosuresHelper.Cnv(args[2], out arg2, typeof(T3));
                    object arg3;
                    ClosuresHelper.Cnv(args[3], out arg3, typeof(T4));
                    object arg4;
                    ClosuresHelper.Cnv(args[4], out arg4, typeof(T5));
                    var ret = func.Invoke((T1)arg0, (T2)arg1, (T3)arg2, (T4)arg3, (T5)arg4);
                    if (ret != null && value != null)
                    {
                        value((Value<TR>)ret);
                    }
                }
            };
        }

        internal void TearDown()
        {
            
        }
    }
}
