using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Remoting
{
	class User : IUser
	{
        private Regulus.Remoting.Ghost.Photon.Agent _Complex { get; set; }

        

        void Regulus.Game.IFramework.Launch()
        {
            _Complex = new Regulus.Remoting.Ghost.Photon.Agent(null);
        }

        void Regulus.Game.IFramework.Shutdown()
        {
            _Complex.Shutdown();
        }

        bool Regulus.Game.IFramework.Update()
        {
            return _Complex.Update();
        }

        Regulus.Remoting.Ghost.IProviderNotice<IVerify> IUser.VerifyProvider
        {
            get { return _Complex.QueryProvider<IVerify>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<IUserStatus> IUser.StatusProvider
        {
            get { return _Complex.QueryProvider<IUserStatus>(); }
        }


        Regulus.Remoting.Ghost.IProviderNotice<ITown> IUser.TownProvider
        {
            get { return _Complex.QueryProvider<ITown>(); }
        }


        Regulus.Remoting.Ghost.IProviderNotice<IAdventure> IUser.AdventureProvider
        {
            get { return _Complex.QueryProvider<IAdventure>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<IAdventureChoice> IUser.AdventureChoiceProvider
        {
            get { return _Complex.QueryProvider<IAdventureChoice>(); }
        }


        

        internal void Connect(string addr)
        {
            _Complex = new Regulus.Remoting.Ghost.Photon.Agent(new Regulus.Remoting.Ghost.Config() { Address = addr, Name = "ExiledPrincesses" });
            var linkState = new Regulus.Remoting.Ghost.LinkState();
            linkState.LinkSuccess += ConnectSuccessEvent;
            linkState.LinkFail += ConnectFailEvent;
            _Complex.Launch(linkState);
        }

        public event Action ConnectSuccessEvent;
        public event Action<string> ConnectFailEvent;


        Regulus.Remoting.Ghost.IProviderNotice<IAdventureIdle> IUser.AdventureIdleProvider
        {
            get { return _Complex.QueryProvider<IAdventureIdle>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<IAdventureGo> IUser.AdventureGoProvider
        {
            get { return _Complex.QueryProvider<IAdventureGo>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<IActor> IUser.ActorProvider
        {
            get { return _Complex.QueryProvider<IActor>(); }
        }


        Regulus.Remoting.Ghost.IProviderNotice<ITeam> IUser.TeamProvider
        {
            get { return _Complex.QueryProvider<ITeam>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<ICombatController> IUser.CombatControllerProvider
        {
            get { return _Complex.QueryProvider<ICombatController>(); }
        }


        Regulus.Remoting.Ghost.IProviderNotice<T> IUser.QueryProivder<T>()
        {
            return _Complex.QueryProvider<T>(); 
        }

        Regulus.Remoting.Ghost.IProviderNotice<Regulus.Remoting.ITime> IUser.TimeProvider
        {
            get { return _Complex.QueryProvider<Regulus.Remoting.ITime>(); }
        }
    }
}

