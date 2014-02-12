using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeysUserConsole.BotStage
{
    class Game : Regulus.Game.IStage<StatusBotController>
    {
        Action<Regulus.Project.SamebestKeys.IPlayer> _OnSupply;
        Regulus.Game.StageLock Regulus.Game.IStage<StatusBotController>.Enter(StatusBotController obj)
        {
            _OnSupply = (player) =>
            {
                player.Ready();
                
                
                
                obj.ToBodyMovements();
            };

            var notify = obj.User.PlayerProvider;
            if(notify.Ghosts.Length > 0 )
            {
                _OnSupply(notify.Ghosts[0]);
            }
            else
                notify.Supply += _OnSupply;

            _Restart = System.DateTime.Now;

            return null;
        }

        void Regulus.Game.IStage<StatusBotController>.Leave(StatusBotController obj)
        {

            var notify = obj.User.PlayerProvider;
            notify.Supply -= _OnSupply;
        }

        System.DateTime _Restart;
        void Regulus.Game.IStage<StatusBotController>.Update(StatusBotController obj)
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
