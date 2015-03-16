using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormulaUserBot
{
    class Bot
    {
        static private long _IdSn;
        private long _Id;
        private string _IPAddress;
        private int _Port;
        private VGame.Project.FishHunter.Formula.IUser _User;

        public Bot(string _IPAddress, int _Port, VGame.Project.FishHunter.Formula.IUser user)
        {
            // TODO: Complete member initialization
            this._IPAddress = _IPAddress;
            this._Port = _Port;
            this._User = user;
            _Id = ++_IdSn ;
            _Begin();
        }
        public void End()
        {
            _User.Remoting.ConnectProvider.Supply -= _Connect;
            _User.VerifyProvider.Supply -= _Verify;
            _User.FishStageQueryerProvider.Supply -= _QueryStage;
            _User.FishStageProvider.Supply -= _FishStage;
        }
        private void _Begin()
        {
            _User.Remoting.ConnectProvider.Supply += _Connect;
            _User.VerifyProvider.Supply += _Verify;
            _User.FishStageQueryerProvider.Supply += _QueryStage;
            _User.FishStageProvider.Supply += _FishStage;
        }

        private void _FishStage(VGame.Project.FishHunter.IFishStage obj)
        {
            VGame.Project.FishHunter.HitRequest request = new VGame.Project.FishHunter.HitRequest();
        }

        private void _QueryStage(VGame.Project.FishHunter.IFishStageQueryer obj)
        {

            _QueryResult(obj.Query(_Id, 0));
        }

        private void _QueryResult(Regulus.Remoting.Value<bool> value)
        {
            
        }

        

        
        private void _Connect(Regulus.Utility.IConnect obj)
        {
            _ConnectResult(obj.Connect(_IPAddress, _Port));
        }

        private void _ConnectResult(Regulus.Remoting.Value<bool> value)
        {
            
        }
        private void _Verify(VGame.Project.FishHunter.IVerify obj)
        {
            _VerifyResult(obj.Login("name", "pw"));
        }

        private void _VerifyResult(Regulus.Remoting.Value<bool> value)
        {
            
        }

        
    }
}
