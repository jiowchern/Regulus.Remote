using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeysUserConsole.BotStage
{
    class Connect : Regulus.Game.IStage<StatusBotController>
    {
        Action _LinkSuccess;
        Regulus.Game.StageLock Regulus.Game.IStage<StatusBotController>.Enter(StatusBotController obj)
        {
            obj.User.Launch();
            
            _LinkSuccess = () => { obj.ToVerify();  };
            obj.User.LinkSuccess += _LinkSuccess;

            return null;
        }

        void Regulus.Game.IStage<StatusBotController>.Leave(StatusBotController obj)
        {
            obj.User.LinkSuccess -= _LinkSuccess;
        }

        void Regulus.Game.IStage<StatusBotController>.Update(StatusBotController obj)
        {
            
        }
    }
}
