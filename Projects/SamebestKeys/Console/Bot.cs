using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    class Bot : Regulus.Utility.IUpdatable
    {
        const string IpAddress = "192.168.40.133";
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
            
        }
        void Regulus.Framework.ILaunched.Launch()
        {
            _ToConnect(IpAddress, Port);    
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
            stage.ResultEvent += () =>
            {
                _ToConnect(IpAddress, Port);
            };

            _Machine.Push(stage);            
        }



        

        
    }
}
