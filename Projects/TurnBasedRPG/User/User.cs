using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.TurnBasedRPG
{
    public class User 
    {
        private Samebest.Remoting.Ghost.Agent _Complex { get; set; }
         
        public event Action LinkSuccess;
        public event Action<string> LinkFail;
        Samebest.Remoting.Ghost.Config _Config;
        public User(Samebest.Remoting.Ghost.Config config )
        {
            _Config = config;
            _VerifyProvider = new Samebest.Remoting.Ghost.Provider<IVerify>();
            _ParkingProvider = new Samebest.Remoting.Ghost.Provider<IParking>();
            _MapInfomationProvider = new Samebest.Remoting.Ghost.Provider<IMapInfomation>();
            _PlayerProvider = new Samebest.Remoting.Ghost.Provider<IPlayer>();
            _ObservedAbilityProvider = new Samebest.Remoting.Ghost.Provider<IObservedAbility>();
            _TimeProvider = new Samebest.Remoting.Ghost.Provider<Samebest.Remoting.ITime>();
            
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
            

            var linkStatu = new Samebest.Remoting.Ghost.LinkState();
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
            _Complex = new Samebest.Remoting.Ghost.Agent(_Config); 

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

        Samebest.Remoting.Ghost.Provider<IVerify> _VerifyProvider;
        public Samebest.Remoting.Ghost.IProviderNotice<IVerify> VerifyProvider
        {
            get { return _VerifyProvider; }
        }

        Samebest.Remoting.Ghost.Provider<IParking> _ParkingProvider;
        public Samebest.Remoting.Ghost.IProviderNotice<IParking> ParkingProvider
        {
            get { return _ParkingProvider; }
        }

        Samebest.Remoting.Ghost.Provider<IMapInfomation> _MapInfomationProvider;
        public Samebest.Remoting.Ghost.IProviderNotice<IMapInfomation> MapInfomationProvider
        {
            get { return _MapInfomationProvider; }
        }

        Samebest.Remoting.Ghost.Provider<IPlayer> _PlayerProvider;
        public Samebest.Remoting.Ghost.IProviderNotice<IPlayer> PlayerProvider
        {
            get { return _PlayerProvider; }
        }

        Samebest.Remoting.Ghost.Provider<IObservedAbility> _ObservedAbilityProvider;        
        public Samebest.Remoting.Ghost.IProviderNotice<IObservedAbility> ObservedAbilityProvider
        {
            get { return _ObservedAbilityProvider; }
        }

        Samebest.Remoting.Ghost.Provider<Samebest.Remoting.ITime> _TimeProvider;
        public Samebest.Remoting.Ghost.IProviderNotice<Samebest.Remoting.ITime> TimeProvider
        {
            get { return _TimeProvider; }
        }
        
    }
}
