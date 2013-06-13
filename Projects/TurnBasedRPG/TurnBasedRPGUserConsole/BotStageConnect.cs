using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPGUserConsole.BotStage
{
    class Connect : Samebest.Game.IStage<StatusBotController>
    {
        Action _LinkSuccess;
        void Samebest.Game.IStage<StatusBotController>.Enter(StatusBotController obj)
        {
            obj.User.Launch();
            
            _LinkSuccess = () => { obj.ToVerify();  };
            obj.User.LinkSuccess += _LinkSuccess;
        }

        void Samebest.Game.IStage<StatusBotController>.Leave(StatusBotController obj)
        {
            obj.User.LinkSuccess -= _LinkSuccess;
        }

        void Samebest.Game.IStage<StatusBotController>.Update(StatusBotController obj)
        {
            
        }
    }
}
