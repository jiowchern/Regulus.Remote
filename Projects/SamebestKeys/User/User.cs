using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.SamebestKeys
{
    using Remoting;

    public interface IConnect
    {
        Regulus.Remoting.Value<bool> Connect(string ipaddr, int port);
    }

    public interface IOnline
    {
        float Ping { get; }
        void Disconnect();

        event Action DisconnectEvent;
    }
    public interface IUser :Regulus.Utility.IUpdatable
    {
        Regulus.Remoting.Ghost.IProviderNotice<IOnline> OnlineProvider { get; }
        Regulus.Remoting.Ghost.IProviderNotice<IConnect> ConnectProvider { get; }
        Regulus.Remoting.Ghost.IProviderNotice<IVerify> VerifyProvider  { get; }
        Regulus.Remoting.Ghost.IProviderNotice<IParking> ParkingProvider { get; }

        Regulus.Remoting.Ghost.IProviderNotice<IMapInfomation> MapInfomationProvider { get; }

        Regulus.Remoting.Ghost.IProviderNotice<IPlayer> PlayerProvider { get; }

        Regulus.Remoting.Ghost.IProviderNotice<IObservedAbility> ObservedAbilityProvider { get; }
        Regulus.Remoting.Ghost.IProviderNotice<ITraversable> TraversableProvider { get; }
        Regulus.Remoting.Ghost.IProviderNotice<ILevelSelector> LevelSelectorProvider { get; }

        Regulus.Remoting.Ghost.IProviderNotice<Regulus.Remoting.ITime> TimeProvider { get; }

        Regulus.Remoting.Ghost.IProviderNotice<IRealmJumper> RealmJumperProvider { get; }

        Regulus.Remoting.Ghost.IProviderNotice<IBelongings> BelongingsProvider { get; }

        Regulus.Remoting.Ghost.IProviderNotice<IIdle> IdleProvider { get; }
        

        Regulus.Remoting.Ghost.IProviderNotice<ISessionStudent> SessionStudentProvider { get; }

        Regulus.Remoting.Ghost.IProviderNotice<ISessionTeacher> SessionTeacherProvider { get; }

        Regulus.Remoting.Ghost.IProviderNotice<ISessionResponse> SessionResponseProvider { get; }

        Regulus.Remoting.Ghost.IProviderNotice<ISessionRequester> SessionRequesterProvider { get; }

        Regulus.Remoting.Ghost.IProviderNotice<ISessionStuff> SessionStuffProvider { get; }


        

        

        
        
    }
}
