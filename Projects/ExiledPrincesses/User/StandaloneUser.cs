using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Standalone
{	
	using Regulus.Project.ExiledPrincesses.Game;


    class Framework : Regulus.Utility.Singleton<Framework>
    {
        Storage _Storage;
        public Regulus.Project.ExiledPrincesses.Game.Zone World { get; private set; }
        public Framework()
        {
            _Storage = new Storage();
            World = new Zone(_Storage);
        }

        public void Update()
        {            
            World.Update();
        }

    }
	class User : IUser
	{
		Regulus.Standalong.Agent _Agent ;
		public User()
		{
            _Agent = new Regulus.Standalong.Agent();
		}

		Regulus.Remoting.Ghost.IProviderNotice<IVerify> IUser.VerifyProvider
		{
			get { return _Agent.QueryProvider<IVerify>(); }
		}

		public void Launch()
		{			
			_Agent.Launch();
            Framework.Instance.World.Enter(_Agent);
		}

		public bool Update()
		{
			_Agent.Update();
            Framework.Instance.Update();
			return true;
		}

		public void Shutdown()
		{
			_Agent.Shutdown();			
		}


        Regulus.Remoting.Ghost.IProviderNotice<IUserStatus> IUser.StatusProvider
        {
            get { return _Agent.QueryProvider<IUserStatus>(); }
        }

        void Regulus.Game.IFramework.Launch()
        {
            throw new NotImplementedException();
        }

        void Regulus.Game.IFramework.Shutdown()
        {
            throw new NotImplementedException();
        }

        bool Regulus.Game.IFramework.Update()
        {
            throw new NotImplementedException();
        }


        Regulus.Remoting.Ghost.IProviderNotice<IParking> IUser.ParkingProvider
        {
            get { return _Agent.QueryProvider<IParking>(); }
        }


        Regulus.Remoting.Ghost.IProviderNotice<IAdventure> IUser.AdventureProvider
        {
            get { return _Agent.QueryProvider<IAdventure>(); }
        }


        Regulus.Remoting.Ghost.IProviderNotice<IReadyCaptureEnergy> IUser.BattleReadyCaptureEnergyProvider
        {
            get { return _Agent.QueryProvider<IReadyCaptureEnergy>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<ICaptureEnergy> IUser.BattleCaptureEnergyProvider
        {
            get { return _Agent.QueryProvider<ICaptureEnergy>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<IEnableChip> IUser.BattleEnableChipProvider
        {
            get { return _Agent.QueryProvider<IEnableChip>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<IDrawChip> IUser.BattleDrawChipProvider
        {
            get { return _Agent.QueryProvider<IDrawChip>(); }
        }


        Regulus.Remoting.Ghost.IProviderNotice<IBattler> IUser.BattleProvider
        {
            get { return _Agent.QueryProvider<IBattler>(); }
        }
    }
}
