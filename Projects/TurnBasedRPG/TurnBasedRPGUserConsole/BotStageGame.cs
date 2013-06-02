using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPGUserConsole.BotStage
{
    class Game : Samebest.Game.IStage<StatusBotController>
    {
        Action<Regulus.Project.TurnBasedRPG.IPlayer> _OnSupply;
        void Samebest.Game.IStage<StatusBotController>.Enter(StatusBotController obj)
        {
            _OnSupply = (player) =>
            {
                player.Ready();
                obj.ToMove();
            };
            var notify = obj.User.Complex.QueryProvider<Regulus.Project.TurnBasedRPG.IPlayer>();
            if(notify.Ghosts.Length > 0 )
            {
                _OnSupply(notify.Ghosts[0]);
            }
            else
                notify.Supply += _OnSupply;

            _Restart = System.DateTime.Now;
        }

        void Samebest.Game.IStage<StatusBotController>.Leave(StatusBotController obj)
        {
            var notify = obj.User.Complex.QueryProvider<Regulus.Project.TurnBasedRPG.IPlayer>();
            notify.Supply -= _OnSupply;
        }

        System.DateTime _Restart;
        void Samebest.Game.IStage<StatusBotController>.Update(StatusBotController obj)
        {
            var time = System.DateTime.Now - _Restart;
            if (time.TotalSeconds > 10)
            {
                /*Console.WriteLine("重新發送 ToGame");
                obj.ToGame();
                _Restart = System.DateTime.Now;*/
            }
        }
    }
}
