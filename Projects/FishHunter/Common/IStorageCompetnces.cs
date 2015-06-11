using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public interface IStorageCompetences
    {
        Regulus.Remoting.Value<VGame.Project.FishHunter.Data.Account.COMPETENCE[]> Query();
        Regulus.Remoting.Value<Guid> QueryForId();
    }
}
