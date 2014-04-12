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
                                    "2jo4jc84hk4g4",
                                    "對話測試",           
                                    " (＠゜▽゜@)ノ",         
                                    "ヾ(●゜▽゜●)♡",
                                    "按Enter做動作切換",
                                    "不要砍我~",
                                    "....",                                                                        
                                    };
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
