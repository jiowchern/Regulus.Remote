using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    class ConnectStorageStage : Regulus.Utility.IStage
    {
        private Storage.IUser _User;
        private string _IpAddress;
        private int _Port;
        public delegate void DoneCallback(bool result);
        public event DoneCallback DoneEvent;
        public ConnectStorageStage(Storage.IUser user, string ipaddress , int port)
        {
            // TODO: Complete member initialization
            this._User = user;
            this._IpAddress = ipaddress;
            _Port = port;
        }

        void Regulus.Utility.IStage.Enter()
        {
            _User.Remoting.ConnectProvider.Supply += _Connect;
        }

        void Regulus.Utility.IStage.Leave()
        {
            _User.Remoting.ConnectProvider.Supply -= _Connect;
        }

        private void _Connect(Regulus.Utility.IConnect obj)
        {            
            var result = obj.Connect(_IpAddress, _Port);
            result.OnValue += (val) => { DoneEvent(val); };
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }
    }
}
