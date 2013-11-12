using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Remoting
{
	class User : IUser
	{
		private Regulus.Remoting.Ghost.Agent _Complex { get; set; }

        

        void Regulus.Game.IFramework.Launch()
        {
            _Complex = new Regulus.Remoting.Ghost.Agent(null);
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


        Regulus.Remoting.Ghost.IProviderNotice<IParking> IUser.ParkingProvider
        {
            get { return _Complex.QueryProvider<IParking>(); }
        }


        Regulus.Remoting.Ghost.IProviderNotice<IAdventure> IUser.AdventureProvider
        {
            get { return _Complex.QueryProvider<IAdventure>(); }
        }


        Regulus.Remoting.Ghost.IProviderNotice<IReadyCaptureEnergy> IUser.BattleReadyCaptureEnergyProvider
        {
            get { return _Complex.QueryProvider<IReadyCaptureEnergy>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<ICaptureEnergy> IUser.BattleCaptureEnergyProvider
        {
            get { return _Complex.QueryProvider<ICaptureEnergy>(); }
        }




        Regulus.Remoting.Ghost.IProviderNotice<IEnableChip> IUser.BattleEnableChipProvider
        {
            get { return _Complex.QueryProvider<IEnableChip>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<IDrawChip> IUser.BattleDrawChipProvider
        {
            get { return _Complex.QueryProvider<IDrawChip>(); }
        }


        Regulus.Remoting.Ghost.IProviderNotice<IBattler> IUser.BattleProvider
        {
            get { return _Complex.QueryProvider<IBattler>(); }
        }

        internal void Connect(string addr)
        {
            _Complex = new Regulus.Remoting.Ghost.Agent(new Regulus.Remoting.Ghost.Config() { Address = addr , Name = "ExiledPrincesses" });
            var linkState = new Regulus.Remoting.Ghost.LinkState();
            linkState.LinkSuccess += ConnectSuccessEvent;
            linkState.LinkFail += ConnectFailEvent;
            _Complex.Launch(linkState);
        }

        public event Action ConnectSuccessEvent;
        public event Action<string> ConnectFailEvent;
    }
}

