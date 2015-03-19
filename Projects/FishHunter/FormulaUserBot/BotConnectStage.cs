using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormulaUserBot
{
    class BotConnectStage : Regulus.Utility.IStage
    {
        public delegate void DoneCallback(VGame.Project.FishHunter.IFishStage stage);
        public event DoneCallback DoneEvent;
        
        private VGame.Project.FishHunter.Formula.IUser _User;
        private string _IPAddress;
        private int _Port;
        private long _Id;
        
        public BotConnectStage(VGame.Project.FishHunter.Formula.IUser user, string ip, int port, long id)
        {
            _Id = id;
            this._User = user;
            this._IPAddress = ip;
            this._Port = port;
        }

        void Regulus.Utility.IStage.Leave()
        {
            _User.Remoting.ConnectProvider.Supply -= _Connect;
            _User.VerifyProvider.Supply -= _Verify;
            _User.FishStageQueryerProvider.Supply -= _Query;
            _User.FishStageProvider.Supply -= _Stage;
        }
        void Regulus.Utility.IStage.Enter()
        {
            _User.FishStageProvider.Supply += _Stage;
            _User.FishStageQueryerProvider.Supply += _Query;
            _User.VerifyProvider.Supply += _Verify;
            _User.Remoting.ConnectProvider.Supply += _Connect;
        }

        private void _Stage(VGame.Project.FishHunter.IFishStage obj)
        {
            DoneEvent(obj);
        }

        private void _Query(VGame.Project.FishHunter.IFishStageQueryer obj)
        {
            _QueryResult(obj.Query(_Id, 1));
        }

        private void _QueryResult(Regulus.Remoting.Value<bool> value)
        {
            value.OnValue += (result) =>
            {
                if (result == false)
                    throw new System.Exception("Stage Query Fail.");                
            };
        }

        private void _Verify(VGame.Project.FishHunter.IVerify obj)
        {
            _VerifyResult(obj.Login("name", "pw"));
        }

        private void _VerifyResult(Regulus.Remoting.Value<bool> value)
        {
            value.OnValue += (result) =>
            {
                if (result == false)
                    throw new System.Exception("Verify Fail.");                    
            };
        }

        private void _Connect(Regulus.Utility.IConnect obj)
        {
            _ConnectResult(obj.Connect(_IPAddress, _Port));
        }

        private void _ConnectResult(Regulus.Remoting.Value<bool> value)
        {
            value.OnValue += (result) => 
            {
                if (result == false)
                    throw new System.Exception("Connect Fail.");
                
            };
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }
    }
}
