using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace VGame.Project.FishHunter.Formula
{
    public interface IUser : Regulus.Utility.IUpdatable
    {
        Regulus.Remoting.User Remoting { get; }

        Regulus.Remoting.Ghost.INotifier<VGame.Project.FishHunter.IVerify> VerifyProvider { get; }
        Regulus.Remoting.Ghost.INotifier<VGame.Project.FishHunter.IFishStageQueryer> FishStageQueryerProvider { get; }        
    }
}
