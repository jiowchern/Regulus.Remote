using System;

namespace Regulus.Projects.SamebestKeys.Standalong
{
	/// <summary>
	/// 單機版
	/// </summary>
    partial class StandalongUser : Regulus.Project.SamebestKeys.IUser
    {
        Regulus.Remoting.Ghost.TProvider<Regulus.Project.SamebestKeys.IConnect> _ConnectProvider;
        Regulus.Remoting.Ghost.TProvider<Regulus.Project.SamebestKeys.IOnline> _OnlineProvider;
        Regulus.Standalong.Agent _Agent;
        Regulus.Game.StageMachine _Machine;
        private Game _Game;
        public StandalongUser(Game game)
        {
            _Game = game;
            _Agent = new Regulus.Standalong.Agent();
            _Machine = new Regulus.Game.StageMachine();            
            _ConnectProvider = new Regulus.Remoting.Ghost.TProvider<Project.SamebestKeys.IConnect>();
            _OnlineProvider = new Regulus.Remoting.Ghost.TProvider<Project.SamebestKeys.IOnline>();
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

        Regulus.Remoting.Ghost.IProviderNotice<Project.SamebestKeys.IRealmJumper> Project.SamebestKeys.IUser.RealmJumperProvider
        {
            get { return _Agent.QueryProvider<Project.SamebestKeys.IRealmJumper>(); }
        }

        Regulus.Remoting.Ghost.IProviderNotice<Project.SamebestKeys.IBelongings> Project.SamebestKeys.IUser.BelongingsProvider
        {
            get { return _Agent.QueryProvider<Project.SamebestKeys.IBelongings>(); }
        }

        bool Utility.IUpdatable.Update()
        {
            
            _Machine.Update();
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            _ToConnect();

            
        }

        private void _ToConnect()
        {
            var stage = new Regulus.Projects.SamebestKeys.ConnectStage(_ConnectProvider , (ip, port) => { return new Regulus.Remoting.Value<bool>(true); });
            stage.ResultEvent += (result) => 
            {
                _ToOnline();
            };
            _Machine.Push(stage);
        }

        private void _ToOnline()
        {
            var stage = new OnlineStage(_Agent, _OnlineProvider,_Game);
            stage.DisconnectEvent += () => 
            {
                _ToConnect();
            };
            
            _Machine.Push(stage);
        }

        void Framework.ILaunched.Shutdown()
        {
            
        }
        
    }

    partial class StandalongUser : Regulus.Project.SamebestKeys.IUser
    {
        class OnlineStage : Regulus.Game.IStage, Regulus.Project.SamebestKeys.IOnline, Regulus.Remoting.Ghost.IGhost
        {
            Guid _Id;
            Regulus.Standalong.Agent _Agent;
            private Regulus.Remoting.Ghost.TProvider<Project.SamebestKeys.IOnline> _Provider;
            Regulus.Utility.Updater _Updater;
            public event Action DisconnectEvent;
            private Game _Game;

            public OnlineStage(Regulus.Standalong.Agent agent , Regulus.Remoting.Ghost.TProvider<Project.SamebestKeys.IOnline> provider , Regulus.Projects.SamebestKeys.Standalong.Game game)
            {
                _Game = game;
                _Updater = new Utility.Updater();
                _Agent = agent;
                _Id = Guid.NewGuid();
                _Provider = provider;
            }            

            float Project.SamebestKeys.IOnline.Ping
            {
                get { return _Agent.Ping; }
            }

            void Project.SamebestKeys.IOnline.Disconnect()
            {
                _Agent.Disconnect();
            }

            event Action Project.SamebestKeys.IOnline.DisconnectEvent
            {
                add { _Agent.BreakEvent += value; }
                remove { _Agent.BreakEvent -= value; }
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

            void Regulus.Game.IStage.Enter()
            {
                (_Provider as Regulus.Remoting.Ghost.IProvider).Add(this);
                (_Provider as Regulus.Remoting.Ghost.IProvider).Ready(_Id);
                _Agent.Launch();
                _Game.Push(_Agent);
                _Agent.BreakEvent += DisconnectEvent;
                
            }

            void Regulus.Game.IStage.Leave()
            {
                
                _Agent.BreakEvent -= DisconnectEvent;
                _Agent.Shutdown();
                (_Provider as Regulus.Remoting.Ghost.IProvider).Remove(_Id);
            }

            void Regulus.Game.IStage.Update()
            {
                _Updater.Update();
                _Agent.Update();
            }
        }
    }
}
