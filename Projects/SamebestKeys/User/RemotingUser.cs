using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Projects.SamebestKeys.Remoting
{
    public partial class RemotingUser : Regulus.Project.SamebestKeys.IUser
    {
        class OnlineStage : Regulus.Game.IStage, Regulus.Project.SamebestKeys.IOnline, Regulus.Remoting.Ghost.IGhost
        {
            Guid _Id;
            
            private Regulus.Remoting.Ghost.TProvider<Project.SamebestKeys.IOnline> _Provider;
            private Regulus.Remoting.Ghost.Native.Agent _Agent;
            public event Action BreakEvent;
            public OnlineStage(Regulus.Remoting.Ghost.TProvider<Project.SamebestKeys.IOnline> provider, Regulus.Remoting.Ghost.Native.Agent agent)
            {
                _Id = Guid.NewGuid();
                this._Provider = provider;
                this._Agent = agent;
                
            }
            void Game.IStage.Enter()
            {
                _Agent.DisconnectEvent += _Agent_DisconnectEvent; 
                (_Provider as Regulus.Remoting.Ghost.IProvider).Add(this);
                (_Provider as Regulus.Remoting.Ghost.IProvider).Ready(_Id);
            }


            public void Disconnect()
            {
                _Agent_DisconnectEvent();
            }
            void _Agent_DisconnectEvent()
            {
                BreakEvent();
                if (_DisconnectEvent != null)
                    _DisconnectEvent();
            }

            void Game.IStage.Leave()
            {
                _Agent.DisconnectEvent -= _Agent_DisconnectEvent; 
                (_Provider as Regulus.Remoting.Ghost.IProvider).Remove(_Id);
            }

            void Game.IStage.Update()
            {
                
                    
            }

            float Project.SamebestKeys.IOnline.Ping
            {
                get { return (float)System.TimeSpan.FromTicks(_Agent.Ping).TotalSeconds; }
            }

            void Project.SamebestKeys.IOnline.Disconnect()
            {
                _Agent.Disconnect();
            }

            void Regulus.Remoting.Ghost.IGhost.OnEvent(string name_event, object[] args)
            {
                throw new NotImplementedException();
            }

            Guid Regulus.Remoting.Ghost.IGhost.GetID()
            {
                return _Id;
            }

            void Regulus.Remoting.Ghost.IGhost.OnProperty(string name, byte[] value)
            {
                throw new NotImplementedException();
            }

            event Action _DisconnectEvent;
            event Action Project.SamebestKeys.IOnline.DisconnectEvent
            {
                add { _DisconnectEvent += value; }
                remove { _DisconnectEvent -= value; }
            }
        }
        
    }
    
    partial class RemotingUser : Regulus.Project.SamebestKeys.IUser
    {

        Regulus.Remoting.Ghost.TProvider<Regulus.Project.SamebestKeys.IConnect> _ConnectProvider;
        Regulus.Remoting.Ghost.TProvider<Regulus.Project.SamebestKeys.IOnline> _OnlineProvider;
        private Regulus.Remoting.Ghost.Native.Agent _Complex;
        private Regulus.Remoting.IAgent _Agent { get { return _Complex; } }

        Regulus.Game.StageMachine _Machine;
        Regulus.Utility.Updater _Updater;
        public RemotingUser()
        {
            _Machine = new Game.StageMachine();            
            _ConnectProvider = new Regulus.Remoting.Ghost.TProvider<Project.SamebestKeys.IConnect>();
            _OnlineProvider = new Regulus.Remoting.Ghost.TProvider<Project.SamebestKeys.IOnline>();
            _Complex = new Regulus.Remoting.Ghost.Native.Agent();
            _Updater = new Utility.Updater();
        }

        

        bool Utility.IUpdatable.Update()
        {
            _Machine.Update();
            _Updater.Update();
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            

            _Updater.Add(_Complex);

            _ToConnect(_Machine);
        }

        private void _ToConnect(Game.StageMachine machine)
        {
            var stage = new ConnectStage(_ConnectProvider, (ipaddr, port) => { return _Complex.Connect(ipaddr, port); });
            stage.ResultEvent += (result)=>
            {
                if (result == true)
                {
                    _ToOnline(machine);
                }
            };
            machine.Push(stage);
        }

        private void _ToOnline(Game.StageMachine machine)
        {
            var stage = new OnlineStage(_OnlineProvider,_Complex);
            
            stage.BreakEvent += () => 
            {                
                _ToConnect(machine);
            };
            machine.Push(stage);
        }

        

        

        void Framework.ILaunched.Shutdown()
        {
            _Machine.Termination();
            _Updater.Shutdown();
        }

        Regulus.Remoting.Ghost.IProviderNotice<Project.SamebestKeys.IOnline> Project.SamebestKeys.IUser.OnlineProvider
        {
            get { return _OnlineProvider; }
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

        Regulus.Remoting.Ghost.IProviderNotice<Project.SamebestKeys.ITraversable> Project.SamebestKeys.IUser.TraversableProvider
        {
            get { return _Agent.QueryProvider<Project.SamebestKeys.ITraversable>(); }
        }


        Regulus.Remoting.Ghost.IProviderNotice<Project.SamebestKeys.ILevelSelector> Project.SamebestKeys.IUser.LevelSelectorProvider
        {
            get { return _Agent.QueryProvider<Project.SamebestKeys.ILevelSelector>(); }
        }
    }
}


