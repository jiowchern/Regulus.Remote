using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPGUserConsole.BotStage
{
    class BodyMovements : Regulus.Game.IStage<StatusBotController>
    {

        Regulus.Game.StageLock Regulus.Game.IStage<StatusBotController>.Enter(StatusBotController obj)
        {
            obj.User.PlayerProvider.Supply += PlayerProvider_Supply;

            return null;
        }
        int _ToMoveTimeSecond;

        Regulus.Utility.TimeCounter _ToMove;
        void PlayerProvider_Supply(TurnBasedRPG.IPlayer obj)
        {
            _ToMove = new Regulus.Utility.TimeCounter();
            var actionStatus = Enum.GetValues(typeof(Regulus.Project.TurnBasedRPG.ActionStatue));
            if (actionStatus.Length > 0)
            {
                var a = (Regulus.Project.TurnBasedRPG.ActionStatue)Regulus.Utility.Random.Next(0, actionStatus.Length - 1);
                obj.BodyMovements(a);
                _ToMoveTimeSecond =  Regulus.Utility.Random.Next(10, 30);

                obj.Say("執行動作" + a.ToString() + "," +  obj.Name + "如此說道");
            }
            
            
            
        }

        void Regulus.Game.IStage<StatusBotController>.Leave(StatusBotController obj)
        {
            obj.User.PlayerProvider.Supply -= PlayerProvider_Supply;
        }

        void Regulus.Game.IStage<StatusBotController>.Update(StatusBotController obj)
        {

            if (_ToMove != null)
            {
                var t = new System.TimeSpan(_ToMove.Ticks).TotalSeconds;
                if (t > _ToMoveTimeSecond)
                {
                    if (Regulus.Utility.Random.Next(0, 1) == 0)
                        obj.ToMove();
                    else
                        obj.ToBodyMovements();
                }
            }
        }
    }
}
