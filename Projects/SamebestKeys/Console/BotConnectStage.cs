using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    class BotConnectStage : Regulus.Game.IStage
    {
        private Regulus.Project.SamebestKeys.IUser _User;
        private string ip;
        private int port;
        public event Action<bool> ResultEvent;
        public BotConnectStage(Regulus.Project.SamebestKeys.IUser _User, string ip, int port)
        {
            // TODO: Complete member initialization
            this._User = _User;
            this.ip = ip;
            this.port = port;
        }
        void Regulus.Game.IStage.Enter()
        {            
            _User.ConnectProvider.Supply += ConnectProvider_Supply;
        }

        void ConnectProvider_Supply(Regulus.Project.SamebestKeys.IConnect obj)
        {
            var val = obj.Connect(ip , port);
            val.OnValue += ResultEvent;
        }



        void Regulus.Game.IStage.Leave()
        {
            _User.ConnectProvider.Supply -= ConnectProvider_Supply;
        }

        void Regulus.Game.IStage.Update()
        {            
        }
    }
}
