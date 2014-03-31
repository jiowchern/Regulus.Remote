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
                                    "xu,41u4rm 5j6xu;61u4",
                                    "卡栽A拉!!!",
                                    "J謀重要",
                                    "這裡是哪?",                                    
                                    "公家機關",
                                    "司法改革",
                                    "有間客棧",
                                    "病換急診",                                    
                                    "去過同學匯了嗎?",
                                    "我昨天有去",
                                    "....",
                                    "我正要去",
                                    "超漂亮",
                                    "超好玩",
                                    "自從我去了同學匯考試都考一百分",};
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
