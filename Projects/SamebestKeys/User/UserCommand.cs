using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Projects.SamebestKeys
{
    using Regulus.Extension;
    class UserCommand
    {
        private Utility.Console.IViewer _View;
        private Utility.Command _Command;
        System.Collections.Generic.Dictionary<object, string[]> _RemoveCommands;
        System.Collections.Generic.Dictionary<object, Action[]> _RemoveEvents;

        public UserCommand(Utility.Console.IViewer view, Utility.Command command)
        {            
            this._View = view;
            this._Command = command;
            _RemoveCommands = new Dictionary<object, string[]>();
            _RemoveEvents = new Dictionary<object, Action[]>();
        }

        internal void Register(Regulus.Project.SamebestKeys.IUser user)
        {

            _Command.Register("Package", () => 
            {
                _View.WriteLine("Read:" + Regulus.Remoting.NetworkStreamReadStage.TotalBytesPerSecond.ToString());
                _View.WriteLine("Write:" + Regulus.Remoting.NetworkStreamWriteStage.TotalBytesPerSecond.ToString()); 
            });


            user.TraversableProvider.Supply += TraversableProvider_Supply;
            user.TraversableProvider.Unsupply += _Unsupply;

            user.PlayerProvider.Supply += PlayerProvider_Supply;
            user.PlayerProvider.Unsupply += _Unsupply;

            user.OnlineProvider.Supply += OnlineProvider_Supply;
            user.OnlineProvider.Unsupply += _Unsupply;

            user.ConnectProvider.Supply += ConnectProvider_Supply;
            user.ConnectProvider.Unsupply += _Unsupply;

            user.VerifyProvider.Supply += VerifyProvider_Supply;
            user.VerifyProvider.Unsupply += _Unsupply;

            user.ParkingProvider.Supply += ParkingProvider_Supply;
            user.ParkingProvider.Unsupply += _Unsupply;

            user.ObservedAbilityProvider.Supply += ObservedAbilityProvider_Supply;
            user.ObservedAbilityProvider.Unsupply += _Unsupply;

            user.LevelSelectorProvider.Supply += _LevelSelectotSupply;
            user.LevelSelectorProvider.Unsupply += _Unsupply;

            user.RealmJumperProvider.Supply += RealmJumperProvider_Supply;
            user.RealmJumperProvider.Unsupply += _Unsupply;

            user.BelongingsProvider.Supply += BelongingsProviderSupply;
            user.BelongingsProvider.Unsupply += _Unsupply;

            user.IdleProvider.Supply += IdleProvider_Supply;
            user.IdleProvider.Unsupply += _Unsupply;
            
        }
        
        

        internal void Unregister(Regulus.Project.SamebestKeys.IUser user)
        {

            user.IdleProvider.Supply -= IdleProvider_Supply;
            user.IdleProvider.Unsupply -= _Unsupply;

            user.BelongingsProvider.Supply -= BelongingsProviderSupply;
            user.BelongingsProvider.Unsupply -= _Unsupply;

            user.RealmJumperProvider.Supply -= RealmJumperProvider_Supply;
            user.RealmJumperProvider.Unsupply -= _Unsupply;


            user.LevelSelectorProvider.Supply -= _LevelSelectotSupply;
            user.LevelSelectorProvider.Unsupply -= _Unsupply;


            user.ObservedAbilityProvider.Supply -= ObservedAbilityProvider_Supply;
            user.ObservedAbilityProvider.Unsupply -= _Unsupply;

            user.PlayerProvider.Supply -= PlayerProvider_Supply;
            user.PlayerProvider.Unsupply -= _Unsupply;

            user.OnlineProvider.Supply -= OnlineProvider_Supply;
            user.OnlineProvider.Unsupply -= _Unsupply;

            user.ConnectProvider.Supply -= ConnectProvider_Supply;
            user.ConnectProvider.Unsupply -= _Unsupply;

            user.VerifyProvider.Supply -= VerifyProvider_Supply;
            user.VerifyProvider.Unsupply -= _Unsupply;

            user.ParkingProvider.Supply -= ParkingProvider_Supply;
            user.ParkingProvider.Unsupply -= _Unsupply;

            user.TraversableProvider.Supply -= TraversableProvider_Supply;
            user.TraversableProvider.Unsupply -= _Unsupply;

            _Command.Unregister("Package");
            foreach (var command in _RemoveCommands)
            {
                foreach (var cmd in command.Value)
                {
                    _Command.Unregister(cmd);
                }
            }
            _RemoveCommands.Clear();

            foreach (var removerEvent in _RemoveEvents)
            {
                var removers = removerEvent.Value;
                foreach (var remover in removers)
                {
                    remover();
                }
            }
            _RemoveEvents.Clear();
        }

        void IdleProvider_Supply(Project.SamebestKeys.IIdle obj)
        {
            _Command.Register<string>("GotoRealm", obj.GotoRealm);
            

            _RemoveCommands.Add(obj, new string[] 
            {
                "GotoRealm" 
            });
        }

        private void BelongingsProviderSupply(Project.SamebestKeys.IBelongings obj)
        {
            _Command.RemotingRegister<int, int>("AddCoins", obj.AddCoins, (coins) =>
            {
                _View.WriteLine("Coins : " + coins.ToString());
            });
            _Command.RemotingRegister<int>("QueryCoins", obj.QueryCoins, (coins) => 
            {
                _View.WriteLine("QueryCoins : " + coins.ToString());
            });

            _RemoveCommands.Add(obj, new string[] 
            {
                "AddCoins" , "QueryCoins"
            });
        }

        void RealmJumperProvider_Supply(Project.SamebestKeys.IRealmJumper obj)
        {
            _Command.Register("QuitRealm", obj.Quit );
            _Command.Register<string>("JumpRealm", obj.Jump);
            _Command.RemotingRegister<string[]>("QueryRealms", obj.Query, (realms) => 
            {
                _View.WriteLine("可玩關卡");
                foreach (var realm in realms)
                {
                    _View.WriteLine(realm);
                }
                
            });

            _RemoveCommands.Add(obj, new string[] 
            {
                "JumpRealm" , "QueryRealms" , "QuitRealm"
            });
        }

        void TraversableProvider_Supply(Project.SamebestKeys.ITraversable obj)
        {
            _Command.RemotingRegister<Regulus.Project.SamebestKeys.Serializable.CrossStatus>("CrossStatus", obj.GetStatus, 
                (cross_info) => 
                {
                    _View.WriteLine("過場中 目標" + cross_info.TargetMap + "(" +cross_info.TargetPosition.ToString() + ")" );
                });

            _Command.Register("Ready" ,obj.Ready);

            _RemoveCommands.Add(obj, new string[] 
            {
                "CrossStatus","Ready"
            });
        }


        void ObservedAbilityProvider_Supply(Project.SamebestKeys.IObservedAbility obj)
        {
            /*obj.ShowActionEvent += (inf) => 
            {
                _View.WriteLine(obj.Name + "移動,方向" + inf.MoveDirection);
            };*/
        }
        void PlayerProvider_Supply(Project.SamebestKeys.IPlayer obj)
        {
            _Command.Register<int>("Cast", obj.Cast);            
            _Command.Register("ChangeMode", obj.ChangeMode);            
            _Command.Register<string , float , float>("Goto", obj.Goto);            
            _Command.Register("Logout", obj.Logout);
            _Command.Register("ExitWorld", obj.ExitWorld);
            _Command.Register<float,float>("SetPosition", obj.SetPosition);
            _Command.Register<int>("SetEnergy", obj.SetEnergy);
            _Command.Register<int>("SetVision", obj.SetVision);
            _Command.Register<float>("SetSpeed", obj.SetSpeed);
            _Command.Register<float,long>("Walk", obj.Walk);
            _Command.Register<float>("Stop", obj.Stop);
            _Command.Register<string>("Say", obj.Say);
            _Command.Register<string>("BodyMovements", (val)=>
            {
                obj.BodyMovements((Project.SamebestKeys.ActionStatue)Enum.Parse(typeof(Project.SamebestKeys.ActionStatue), val));
            });

            

            _RemoveCommands.Add(obj, new string[] 
            {
                "Logout","ExitWorld","SetPosition","SetVision",
                "SetSpeed","Walk","Stop","Say","BodyMovements",
                "Goto","ChangeMode" , "Cast"
            });
        }


        void _LevelSelectotSupply(Project.SamebestKeys.ILevelSelector obj)
        {
            
            
            _Command.Register("Back", obj.Back );
            _Command.RemotingRegister<string[]>("Levels", obj.QueryLevels, (levels) => 
            {
                _View.Write("關卡:");
                foreach (var level in levels)
                {
                    _View.Write("["+level+"]");
                }
                _View.WriteLine("");
            });
            _Command.RemotingRegister<string, bool>("Select", obj.Select, (result) => { _View.WriteLine("關卡選擇" + result.ToString() ); });

            _RemoveCommands.Add(obj, new string[] 
            {
                "Ping"  ,"Disconnect"
            });
        }

        void OnlineProvider_Supply(Project.SamebestKeys.IOnline obj)
        {
            _Command.Register("Ping", () => { _View.WriteLine(obj.Ping.ToString()); });
            _Command.Register("Disconnect" ,obj.Disconnect);

            _RemoveCommands.Add(obj, new string[] 
            {
                "Ping"  ,"Disconnect"
            });
        }

        void ParkingProvider_Supply(Project.SamebestKeys.IParking obj)
        {
            _Command.Register("Back", obj.Back);

            _Command.RemotingRegister<string, bool>("CreateActor",
                (name) => { return obj.CreateActor(new Project.SamebestKeys.Serializable.EntityLookInfomation() { Name = name }); },
                (result)=>{ _View.WriteLine("角色建立" + result.ToString() );});

            _Command.RemotingRegister<string, bool>("CheckActorName",
                (name) => { return obj.CheckActorName(name); },
                (result) => { _View.WriteLine("角色建立" + result.ToString()); });
            _Command.RemotingRegister<string,Regulus.Project.SamebestKeys.Serializable.EntityLookInfomation[]>("DestroyActor", obj.DestroyActor,
                (actors) =>
                {
                    foreach (var actor in actors)
                    {
                        _View.WriteLine("角色 : "+ actor.Name);
                    }
                });

            _Command.RemotingRegister<Regulus.Project.SamebestKeys.Serializable.EntityLookInfomation[]>("QueryActors", obj.QueryActors,
                (actors) =>
                {
                    foreach (var actor in actors)
                    {
                        _View.WriteLine("角色 : " + actor.Name);
                    }
                });

            _Command.RemotingRegister<string, string>("Select", obj.Select, (result) =>
                {
                    _View.WriteLine("goto : " + result);
                });
            

            _RemoveCommands.Add(obj, new string[] 
            {
                "Back" , "CreateActor" , "CheckActorName" , "DestroyActor" , "QueryActors" , "Select"
            });
        }

        void VerifyProvider_Supply(Project.SamebestKeys.IVerify verify)
        {
            _Command.RemotingRegister<string, string, bool>("CreateAccount", verify.CreateAccount, (result) =>
            {
                if (result)
                {
                    _View.WriteLine("建立成功");
                }
                else
                    _View.WriteLine("建立失敗");
            });

            _Command.RemotingRegister<string, string, Regulus.Project.SamebestKeys.LoginResult>("Login", verify.Login, (result) =>
            {
                if (result == Project.SamebestKeys.LoginResult.Success)
                {
                    _View.WriteLine("登入成功");
                }
                else if (result == Project.SamebestKeys.LoginResult.Error)
                    _View.WriteLine("登入失敗");
                else if (result == Project.SamebestKeys.LoginResult.RepeatLogin)
                    _View.WriteLine("重複登入");
            });
            _Command.Register("Quit", verify.Quit);


            _RemoveCommands.Add(verify, new string[] 
            {
                "CreateAccount"  , "Quit" , "Login"
            });
        }
       

        void ConnectProvider_Supply(Project.SamebestKeys.IConnect connect)
        {
            _Command.RemotingRegister<string, int, bool>("Connect", connect.Connect, (result) => 
            {
                if (result)
                {
                    _View.WriteLine("連線成功");
                }
                else
                    _View.WriteLine("連線失敗");
            });

            _RemoveCommands.Add(connect, new string[] 
            {
                "Connect" 
            });
        }


        void _Unsupply<T>(T obj)
        {
            
            string[] commands;
            if (_RemoveCommands.TryGetValue(obj, out commands))
            {
                foreach (var command in commands)
                {
                    _Command.Unregister(command);
                }
            }

            Action[] removers;
            if (_RemoveEvents.TryGetValue(obj, out removers))
            {
                foreach (var remover in removers)
                {
                    remover();
                }
            }

            _RemoveCommands.Remove(obj);
            _RemoveEvents.Remove(obj);
        }
    }
}


interface ICodeStyle
{
    void PureVirtualFunction(int arg_1);
}

class CodeStyle : ICodeStyle
{    
    public int PublicProperty  { get; private set; }        
    int _PrivateMember; // field

    public void PublicFunction(int arg_data1, int arg_data2)
    {
        int localData = 0;
    }

    void _PrivateFunction(int arg_data1, int arg_data2)
    {

    } 

    public delegate void OnDelegateSample(int arg_1);
    public event OnDelegateSample DelegateSampleEvent;
    private event OnDelegateSample _PrivateDelegateSampleEvent;

    /*介面繼承以明確實作為主*/
    void ICodeStyle.PureVirtualFunction(int arg_1)
    {
        PureVirtualFunction(arg_1);
    }

    public void PureVirtualFunction(int arg_1) 
    { 

    }
}

