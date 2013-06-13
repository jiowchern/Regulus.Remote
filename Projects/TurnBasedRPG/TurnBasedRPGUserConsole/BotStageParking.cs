using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPGUserConsole.BotStage
{
    class Parking : Samebest.Game.IStage<StatusBotController>
    {
        Action<Regulus.Project.TurnBasedRPG.IParking> _OnSupply;
        void Samebest.Game.IStage<StatusBotController>.Enter(StatusBotController obj)
        {
            _OnSupply = (patking) =>
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

            var notify = obj.User.ParkingProvider;
            if (notify.Ghosts.Length > 0)
            {
                _OnSupply(notify.Ghosts[0]);
            }
            else
            {
                notify.Supply += _OnSupply;
            }
            _Restart = System.DateTime.Now;
        }

        void Samebest.Game.IStage<StatusBotController>.Leave(StatusBotController obj)
        {
            var notify = obj.User.ParkingProvider;
            notify.Supply -= _OnSupply;
        }

        System.DateTime _Restart;
        void Samebest.Game.IStage<StatusBotController>.Update(StatusBotController obj)
        {
            var time = System.DateTime.Now - _Restart;
            if (time.TotalSeconds > 10)
            {
                /*obj.ToParking();
                Console.WriteLine("重新發送 ToParking");
                _Restart = System.DateTime.Now;*/
            }
        }
    }
}
