using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    class BotTalkStage: Regulus.Game.IStage
    {
        private Regulus.Project.SamebestKeys.IPlayer _Player;
        

        public event Action DoneEvent;
        public BotTalkStage(Regulus.Project.SamebestKeys.IPlayer _Player)
        {
            
            this._Player = _Player;            
        }


        static string[] _Messages = { 
                                    "這是測試",
                                    "公家機關",
                                    "司法改革",
                                    "有間客棧",
                                    "病入膏肓",};
        void Regulus.Game.IStage.Enter()
        {

            _Player.Say(_Messages[Regulus.Utility.Random.Next(0, _Messages.Length)]);
            DoneEvent();
        }

        void Regulus.Game.IStage.Leave()
        {
                     
        }

        void Regulus.Game.IStage.Update()
        {
            
        }
    }
}
