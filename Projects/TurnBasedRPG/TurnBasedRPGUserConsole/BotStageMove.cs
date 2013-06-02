using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPGUserConsole.BotStage
{
    class Move : Samebest.Game.IStage<StatusBotController>
    {
        int _IdleTime;
        void Samebest.Game.IStage<StatusBotController>.Enter(StatusBotController obj)
        {
            var notify = obj.User.Complex.QueryProvider<Regulus.Project.TurnBasedRPG.IPlayer>();
            if (notify.Ghosts.Length > 0)
            {
                notify.Ghosts[0].SetPosition( Regulus.Utility.Random.Instance.R.Next(0 , 100) , Regulus.Utility.Random.Instance.R.Next(0 , 100));
                
            }

            _Logout = System.DateTime.Now;
            _IdleTime = Regulus.Utility.Random.Instance.R.Next(10, 60);
        }

        System.DateTime _Logout;
        void Samebest.Game.IStage<StatusBotController>.Leave(StatusBotController obj)
        {
            
        }

        void Samebest.Game.IStage<StatusBotController>.Update(StatusBotController obj)
        {
            if ((System.DateTime.Now - _Logout).TotalSeconds > _IdleTime)
            {
                var notify = obj.User.Complex.QueryProvider<Regulus.Project.TurnBasedRPG.IPlayer>();
                if (notify.Ghosts.Length > 0)
                    notify.Ghosts[0].Logout();
                obj.ToVerify();
            }
        }
    }
}
