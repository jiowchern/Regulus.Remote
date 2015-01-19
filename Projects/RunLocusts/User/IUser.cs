using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imdgame.RunLocusts
{
    public interface IUser 
        : Regulus.Utility.IUpdatable
    {
        Regulus.Remoting.Ghost.IProviderNotice<Regulus.Utility.IConnect> ConnectProvider { get; }
        Regulus.Remoting.Ghost.IProviderNotice<Regulus.Utility.IOnline> OnlineProvider { get; }
    }
}
