using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPGUserConsole.BotStage
{
    class Parking : Samebest.Game.IStage<StatusBotController>
    {

        void Samebest.Game.IStage<StatusBotController>.Enter(StatusBotController obj)
        {
            Action<Regulus.Project.TurnBasedRPG.Common.IParking> onSupply = (patking) =>
            {
                var val = patking.Select(obj.Name);
                val.OnValue += ( res )=>
                {
                    if (res)
                    {
                        Console.WriteLine("角色{0}選擇成功.", obj.Name);
                        obj.ToGame();
                    }
                    else
                    { 
                        patking.CreateActor( new TurnBasedRPG.Serializable.EntityLookInfomation() { Name = obj.Name });
                        obj.ToParking();    
                    }
                };
            };
            var notify = obj.User.Complex.QueryProvider<Regulus.Project.TurnBasedRPG.Common.IParking>();
            if (notify.Ghosts.Length > 0)
            {
                onSupply(notify.Ghosts[0]);
            }
            else
            {
                notify.Supply += onSupply;
            }
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
                obj.ToParking();
                Console.WriteLine("重新發送 ToParking");
                _Restart = System.DateTime.Now;
            }
        }
    }
}
