using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    class Bot : Regulus.Utility.IUpdatable
    {
        //const string IpAddress = "192.168.40.133";
        const string IpAddress = "114.34.90.217";
        //const string IpAddress = "127.0.0.1";
        //const string IpAddress = "23.97.70.8";
        //const string IpAddress = "192.168.40.87";
        const int Port = 12345;
        private Regulus.Project.SamebestKeys.IUser _User;        
        Regulus.Game.StageMachine _Machine;
        
        string _Account;
        public Bot(Regulus.Project.SamebestKeys.IUser user,string account)
        {
            _Machine = new Regulus.Game.StageMachine();
            this._User = user;
            _Account = account;
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Machine.Update();
            return true;
        }
        void Regulus.Framework.ILaunched.Shutdown()
        {
            _User.OnlineProvider.Supply -= OnlineProvider_Supply;
        }
        void Regulus.Framework.ILaunched.Launch()
        {
            _ToConnect(IpAddress, Port);
            _User.OnlineProvider.Supply += OnlineProvider_Supply;
        }

        Action _OnlineOnDisconnect;
        void OnlineProvider_Supply(Regulus.Project.SamebestKeys.IOnline obj)
        {
            _OnlineOnDisconnect = () => 
            {
                obj.DisconnectEvent -= _OnlineOnDisconnect;
                _ToConnect(IpAddress, Port);
            };
            obj.DisconnectEvent += _OnlineOnDisconnect;
        }

        

        private void _ToConnect(string ip, int port)
        {
            var stage = new BotConnectStage(_User , ip ,port );
            stage.ResultEvent += (result) =>
            {
                if (result)
                {
                    _ToVerify(_Account);
                }
                else
                {
                    _ToConnect(IpAddress, Port);
                }
            };
            _Machine.Push(stage);
        }

        private void _ToVerify(string account)
        {
            var stage = new BotVerifyStage(_User , account );
            stage.ResultEvent += (result) =>
            {                
                if (result == Regulus.Project.SamebestKeys.LoginResult.Success)
                {
                    _ToSelectActor(account);
                }
                else if (result == Regulus.Project.SamebestKeys.LoginResult.Error)
                {
                    _ToCreateAccount(account);
                }
                else if (result == Regulus.Project.SamebestKeys.LoginResult.RepeatLogin)
                {
                    _ToVerify(account);
                }
            };
            _Machine.Push(stage);
        }

        private void _ToCreateAccount(string account)
        {
            var stage = new BotCreateAccountStage(_User, account);
            stage.ResultEvent += (result) =>
            {
                if (result)
                {
                    _ToVerify(account);
                }
                else
                {
                    throw new SystemException("建立不存在的帳號");
                }
            };
            _Machine.Push(stage);            
        }

        private void _ToSelectActor(string account)
        {
            var stage = new BotSelectActorStage(_User , account );
            stage.ResultEvent += (result) =>
            {
                if (result)
                {
                    _ToMap();
                }
                else
                {
                    _ToCreateActor(account);
                }
            };
            _Machine.Push(stage);            
        }

        private void _ToCreateActor(string account)
        {
            var stage = new BotCreateActorStage(_User, account);
            stage.ResultEvent += (result) =>
            {
                if (result)
                {
                    _ToSelectActor(account);
                }
                else
                {
                    throw new SystemException("建立不存在的人物");
                }

            };

            _Machine.Push(stage);            
        }

        private void _ToMap()
        {
            var stage = new BotMapStage(_User);
            stage.ResultEvent += (result) =>
            {
                if(result == BotMapStage.Result.Connect)
                    _ToConnect(IpAddress, Port);
                if (result == BotMapStage.Result.Reset)
                    _ToMap();
            };

            _Machine.Push(stage);            
        }







        
    }
}
