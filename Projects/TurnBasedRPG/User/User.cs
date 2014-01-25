using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.TurnBasedRPG
{
    public class User 
    {
        private Regulus.Remoting.Ghost.Photon.Agent _Complex { get; set; }
         
        public event Action LinkSuccess;
        public event Action<string> LinkFail;
        Regulus.Remoting.Ghost.Config _Config;
        public User(Regulus.Remoting.Ghost.Config config )
        {
            _Config = config;
            _VerifyProvider = new Regulus.Remoting.Ghost.Provider<IVerify, Regulus.Remoting.Ghost.Photon.Agent>();
            _ParkingProvider = new Regulus.Remoting.Ghost.Provider<IParking, Regulus.Remoting.Ghost.Photon.Agent>();
            _MapInfomationProvider = new Regulus.Remoting.Ghost.Provider<IMapInfomation, Regulus.Remoting.Ghost.Photon.Agent>();
            _PlayerProvider = new Regulus.Remoting.Ghost.Provider<IPlayer, Regulus.Remoting.Ghost.Photon.Agent>();
            _ObservedAbilityProvider = new Regulus.Remoting.Ghost.Provider<IObservedAbility, Regulus.Remoting.Ghost.Photon.Agent>();
            _TimeProvider = new Regulus.Remoting.Ghost.Provider<Regulus.Remoting.ITime, Regulus.Remoting.Ghost.Photon.Agent>();
            
        }
        public long GetPing(int idx)
        {
            if (idx == 0 && _Complex != null)
                return _Complex.Ping;
            return -1 ;
        }
        
        public void Launch()
        {

            if (_Complex != null)
            {
                _TimeProvider.Remove(_Complex);
                _VerifyProvider.Remove(_Complex);
                _ParkingProvider.Remove(_Complex);
                _MapInfomationProvider.Remove(_Complex);
                _PlayerProvider.Remove(_Complex);
                _ObservedAbilityProvider.Remove(_Complex);
            }
            

            var linkStatu = new Regulus.Remoting.Ghost.LinkState();
            linkStatu.LinkSuccess += () =>
            {
                if (LinkSuccess != null)
                    LinkSuccess();
            };
            linkStatu.LinkFail += (s) =>            
            {
                if(LinkFail != null)
                    LinkFail(s);
            };
            _Complex = new Regulus.Remoting.Ghost.Photon.Agent(_Config); 

            _Complex.Launch(linkStatu);
            _VerifyProvider.Add(_Complex);
            _ParkingProvider.Add(_Complex);
            _MapInfomationProvider.Add(_Complex);
            _PlayerProvider.Add(_Complex);
            _ObservedAbilityProvider.Add(_Complex);
            _TimeProvider.Add(_Complex);           
        }

        public  bool Update()
        {
            if (_Complex != null)
                return _Complex.Update();
            return true;
        }

        public void Shutdown()
        {
            if (_Complex != null)
                _Complex.Shutdown();
            _Complex = null;
        }

        Regulus.Remoting.Ghost.Provider<IVerify, Regulus.Remoting.Ghost.Photon.Agent> _VerifyProvider;
        public Regulus.Remoting.Ghost.IProviderNotice<IVerify> VerifyProvider
        {
            get { return _VerifyProvider; }
        }

        Regulus.Remoting.Ghost.Provider<IParking, Regulus.Remoting.Ghost.Photon.Agent> _ParkingProvider;
        public Regulus.Remoting.Ghost.IProviderNotice<IParking> ParkingProvider
        {
            get { return _ParkingProvider; }
        }

        Regulus.Remoting.Ghost.Provider<IMapInfomation, Regulus.Remoting.Ghost.Photon.Agent> _MapInfomationProvider;
        public Regulus.Remoting.Ghost.IProviderNotice<IMapInfomation> MapInfomationProvider
        {
            get { return _MapInfomationProvider; }
        }

        Regulus.Remoting.Ghost.Provider<IPlayer, Regulus.Remoting.Ghost.Photon.Agent> _PlayerProvider;
        public Regulus.Remoting.Ghost.IProviderNotice<IPlayer> PlayerProvider
        {
            get { return _PlayerProvider; }
        }

        Regulus.Remoting.Ghost.Provider<IObservedAbility, Regulus.Remoting.Ghost.Photon.Agent> _ObservedAbilityProvider;        
        public Regulus.Remoting.Ghost.IProviderNotice<IObservedAbility> ObservedAbilityProvider
        {
            get { return _ObservedAbilityProvider; }
        }

        Regulus.Remoting.Ghost.Provider<Regulus.Remoting.ITime, Regulus.Remoting.Ghost.Photon.Agent> _TimeProvider;
        public Regulus.Remoting.Ghost.IProviderNotice<Regulus.Remoting.ITime> TimeProvider
        {
            get { return _TimeProvider; }
        }
        
    }
}
