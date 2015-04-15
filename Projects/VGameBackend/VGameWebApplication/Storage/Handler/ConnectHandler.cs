using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGameWebApplication.Storage.Handler
{
    class Connect : WaitSomething<bool>
    {
        private VGame.Project.FishHunter.Storage.IUser user;
        private string ip;
        private int port;

        bool _Result;        
        public Connect(VGame.Project.FishHunter.Storage.IUser user, string ip, int port)
        {
            // TODO: Complete member initialization
            this.user = user;
            this.ip = ip;
            this.port = port;
        }

        protected override bool End()
        {
            user.Remoting.ConnectProvider.Supply -= ConnectProvider_Supply;
            return _Result;
        }

        protected override void Start()
        {
            user.Remoting.ConnectProvider.Supply += ConnectProvider_Supply;
        }

        void ConnectProvider_Supply(Regulus.Utility.IConnect obj)
        {
            var result = obj.Connect(ip, port);
            result.OnValue += result_OnValue;
        }

        void result_OnValue(bool obj)
        {
            _Result = obj;
            Stop();
        }



        public bool Result { get { return _Result; } }
    }
}
