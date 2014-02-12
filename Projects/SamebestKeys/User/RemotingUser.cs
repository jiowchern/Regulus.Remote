using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Projects.SamebestKeys.Remoting
{
    class RemotingUser : Regulus.Project.SamebestKeys.IUser
    {
        Connect _Connecter;
        Regulus.Remoting.Ghost.TProvider<Regulus.Project.SamebestKeys.IConnect> _ConnectProvider;
        private Regulus.Remoting.Ghost.Native.Agent _Complex;
        private Regulus.Remoting.IAgent _Agent { get { return _Complex; } }


        Regulus.Utility.Updater _Updater;
        public RemotingUser()
        {
            _Connecter = new Connect();
            _ConnectProvider = new Regulus.Remoting.Ghost.TProvider<Project.SamebestKeys.IConnect>();
            _Complex = new Regulus.Remoting.Ghost.Native.Agent();
            _Updater = new Utility.Updater();
        }

        Regulus.Remoting.Ghost.IProviderNotice<Project.SamebestKeys.IConnect> Project.SamebestKeys.IUser.ConnectProvider
        {
            get { return _ConnectProvider; }
        }

        Regulus.Remoting.Ghost.IProviderNotice<Project.SamebestKeys.IVerify> Project.SamebestKeys.IUser.VerifyProvider
        {
            get { return _Agent.QueryProvider<Project.SamebestKeys.IVerify>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<Project.SamebestKeys.IParking> Project.SamebestKeys.IUser.ParkingProvider
        {
            get { return _Agent.QueryProvider<Project.SamebestKeys.IParking>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<Project.SamebestKeys.IMapInfomation> Project.SamebestKeys.IUser.MapInfomationProvider
        {
            get { return _Agent.QueryProvider<Project.SamebestKeys.IMapInfomation>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<Project.SamebestKeys.IPlayer> Project.SamebestKeys.IUser.PlayerProvider
        {
            get { return _Agent.QueryProvider<Project.SamebestKeys.IPlayer>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<Project.SamebestKeys.IObservedAbility> Project.SamebestKeys.IUser.ObservedAbilityProvider
        {
            get { return _Agent.QueryProvider<Project.SamebestKeys.IObservedAbility>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<Regulus.Remoting.ITime> Project.SamebestKeys.IUser.TimeProvider
        {
            get { return _Agent.QueryProvider<Regulus.Remoting.ITime>(); }
        }

        bool Utility.IUpdatable.Update()
        {
            _Updater.Update();
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            _Connecter.ConnectedEvent += _OnConnect;
            (_ConnectProvider as Regulus.Remoting.Ghost.IProvider).Add(_Connecter);
            (_ConnectProvider as Regulus.Remoting.Ghost.IProvider).Ready(_Connecter.Id);

            _Updater.Add(_Complex);
        }

        void _OnConnect(string ipaddr, int port, Regulus.Remoting.Value<bool> result)
        {
            var value = _Complex.Connect(ipaddr, port );
            value.OnValue += (ret) =>
            {
                result.SetValue(ret);
            };
        }

        void Framework.ILaunched.Shutdown()
        {
            (_ConnectProvider as Regulus.Remoting.Ghost.IProvider).Remove(_Connecter.Id);
        }
    }
}


