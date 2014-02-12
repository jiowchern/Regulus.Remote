
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeysUserConsole
{
    namespace BotStage
    {
        public class Verify : Regulus.Game.IStage<StatusBotController>
        {
            Action<Regulus.Project.SamebestKeys.IVerify> _OnSupply;
            Regulus.Game.StageLock Regulus.Game.IStage<StatusBotController>.Enter(StatusBotController obj)
            {
                _OnSupply = (verify) =>
                {
                    var resule = verify.Login(obj.Name, "1");
                    resule.OnValue += (res) =>
                    {
                        if (res == SamebestKeys.LoginResult.Success )
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



                var notify = obj.User.VerifyProvider;
                if (notify.Ghosts.Length > 0)
                {
                    _OnSupply(notify.Ghosts[0]);
                }
                else
                {
                    notify.Supply += _OnSupply;
                }
                _Restart = System.DateTime.Now;

                return null;
            }

            

            void Regulus.Game.IStage<StatusBotController>.Leave(StatusBotController obj)
            {

                var notify = obj.User.VerifyProvider;
                notify.Supply -= _OnSupply;
            }

            System.DateTime _Restart;
            void Regulus.Game.IStage<StatusBotController>.Update(StatusBotController obj)
            {
                var time =System.DateTime.Now - _Restart;
                if (time.TotalSeconds > 10)
                {
                    /*obj.ToVerify();
                    Console.WriteLine("重新發送 ToVerify");
                    _Restart = System.DateTime.Now;*/
                }
            }
        }
    }
}
