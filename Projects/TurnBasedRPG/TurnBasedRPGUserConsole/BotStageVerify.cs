
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPGUserConsole
{
    namespace BotStage
    {
        public class Verify : Samebest.Game.IStage<StatusBotController>
        {
            void Samebest.Game.IStage<StatusBotController>.Enter(StatusBotController obj)
            {
                Action<Regulus.Project.TurnBasedRPG.Common.IVerify> onSupply = (verify) =>
                {
                    var resule = verify.Login(obj.Name, "1");
                    resule.OnValue += (res) =>
                    {
                        if (res)
                        {
                            obj.ToParking();
                        }
                        else
                        {
                            verify.CreateAccount(obj.Name, "1");
                            Console.WriteLine("建立帳號 " + obj.Name);
                            obj.ToVerify();
                        }
                    };
                };


                var notify = obj.User.Complex.QueryProvider<Regulus.Project.TurnBasedRPG.Common.IVerify>();
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
                var time =System.DateTime.Now - _Restart;
                if (time.TotalSeconds > 10)
                {
                    obj.ToVerify();
                    Console.WriteLine("重新發送 ToVerify");
                    _Restart = System.DateTime.Now;
                }
            }
        }
    }
}
