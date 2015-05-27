using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGame.Project.FishHunter.Storage
{
    public interface IUser : Regulus.Utility.IUpdatable
    {
        Regulus.Remoting.User Remoting { get; }

        Regulus.Remoting.Ghost.INotifier<VGame.Project.FishHunter.IVerify> VerifyProvider { get; }

        Regulus.Remoting.Ghost.INotifier<VGame.Project.FishHunter.IStorageCompetnces> StorageCompetncesProvider { get; }

        Regulus.Remoting.Ghost.INotifier<T> QueryProvider<T>();
    }
}
