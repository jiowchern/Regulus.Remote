using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPGUserConsole.BotStage
{
    class Move : Regulus.Game.IStage<StatusBotController>
    {
        int _IdleTime;
        Regulus.Game.StageLock Regulus.Game.IStage<StatusBotController>.Enter(StatusBotController obj)
        {

            var notify = obj.User.PlayerProvider;
            if (notify.Ghosts.Length > 0)
            {                
                notify.Ghosts[0].Walk(Regulus.Utility.Random.Next(0 ,360));                
            }

            _Logout = System.DateTime.Now;
            _IdleTime =  Regulus.Utility.Random.Instance.R.Next(5,10 );

            return null;
        }

        System.DateTime _Logout;
        void Regulus.Game.IStage<StatusBotController>.Leave(StatusBotController obj)
        {
            
        }

        void Regulus.Game.IStage<StatusBotController>.Update(StatusBotController obj)
        {
            if ((System.DateTime.Now - _Logout).TotalSeconds > _IdleTime)
            {
                var notify = obj.User.PlayerProvider;
                if (notify.Ghosts.Length > 0)
                    notify.Ghosts[0].Logout();
                obj.ToVerify();
            }
        }
    }
}
