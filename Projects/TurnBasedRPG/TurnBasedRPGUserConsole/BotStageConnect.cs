using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPGUserConsole.BotStage
{
    class Connect : Regulus.Utility.IStage<StatusBotController>
    {
        Action _LinkSuccess;
        Regulus.Utility.StageLock Regulus.Utility.IStage<StatusBotController>.Enter(StatusBotController obj)
        {
            obj.User.Launch();
            
            _LinkSuccess = () => { obj.ToVerify();  };
            obj.User.LinkSuccess += _LinkSuccess;

            return null;
        }

        void Regulus.Utility.IStage<StatusBotController>.Leave(StatusBotController obj)
        {
            obj.User.LinkSuccess -= _LinkSuccess;
        }

        void Regulus.Utility.IStage<StatusBotController>.Update(StatusBotController obj)
        {
            
        }
    }
}
