using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public class DummyFrature : IAccountFinder , IFishStageQueryer
    {
        Regulus.Remoting.Value<Data.Account> IAccountFinder.FindAccountByName(string id)
        {
            return new Data.Account { Id = Guid.NewGuid(), Password = "pw", Name = id, Competnce = Data.Account.COMPETENCE.ALL };
        }


        Regulus.Remoting.Value<Data.Account> IAccountFinder.FindAccountById(Guid accountId)
        {
            return new Data.Account { Id = accountId, Password = "pw", Name = "name", Competnce = Data.Account.COMPETENCE.ALL };
        }

        Regulus.Remoting.Value<IFishStage> IFishStageQueryer.Query(long player_id, byte fish_stage)
        {
            return new VGame.Project.FishHunter.Formula.FishStage(player_id, fish_stage);
        }
    }
}
