using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormulaUserBot
{
    class Bot : Regulus.Utility.IUpdatable
    {
        static private long _IdSn;
        private long _Id;
        private string _IPAddress;
        private int _Port;
        private VGame.Project.FishHunter.Formula.IUser _User;


        Regulus.Utility.StageMachine _Machine;
        public Bot(string _IPAddress, int _Port, VGame.Project.FishHunter.Formula.IUser user)
        {
            // TODO: Complete member initialization
            this._IPAddress = _IPAddress;
            this._Port = _Port;
            this._User = user;
            _Id = ++_IdSn ;
            _Machine = new Regulus.Utility.StageMachine();
        }
        

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Machine.Update();
            return true;
        }

        void Regulus.Framework.IBootable.Shutdown()
        {
            _Machine.Termination();
        }

        void Regulus.Framework.IBootable.Launch()
        {
            _ToConnect();
        }

        private void _ToConnect()
        {
            var stage = new BotConnectStage(_User, _IPAddress, _Port, _Id);
            stage.DoneEvent += _ToPlay;
            _Machine.Push(stage);
        }

        private void _ToPlay(VGame.Project.FishHunter.IFishStage fish_stage)
        {
            var stage = new BotPlayStage(fish_stage);
            stage.DoneEvent += _ToConnect;
            _Machine.Push(stage);
        }

        

        
    }
}
