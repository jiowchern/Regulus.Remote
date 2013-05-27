using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPGUserConsole.BotStage
{
    class Game : Samebest.Game.IStage<StatusBotController>
    {

        void Samebest.Game.IStage<StatusBotController>.Enter(StatusBotController obj)
        {
            Action<Regulus.Project.TurnBasedRPG.Common.IPlayer> onSupply = (player) =>
            {
                player.Logout();
                obj.ToVerify();
            };
            var notify = obj.User.Complex.QueryProvider<Regulus.Project.TurnBasedRPG.Common.IPlayer>();
            if(notify.Ghosts.Length > 0 )
            {
                onSupply(notify.Ghosts[0]);
            }
            else
                notify.Supply += onSupply;

            _Restart = System.DateTime.Now;
        }

        void Samebest.Game.IStage<StatusBotController>.Leave(StatusBotController obj)
        {
            
        }

        System.DateTime _Restart;
        void Samebest.Game.IStage<StatusBotController>.Update(StatusBotController obj)
        {
            var time = System.DateTime.Now - _Restart;
            if (time.TotalSeconds > 10)
            {
                Console.WriteLine("重新發送 ToGame");
                obj.ToGame();
                _Restart = System.DateTime.Now;
            }
        }
    }
}
