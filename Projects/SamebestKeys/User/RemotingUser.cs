using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Projects.SamebestKeys.Remoting
{
    partial class RemotingUser : Regulus.Project.SamebestKeys.IUser
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

                (_Provider as Regulus.Remoting.Ghost.IProvider).Add(this);
                (_Provider as Regulus.Remoting.Ghost.IProvider).Ready(_Id);
            }

            void Game.IStage.Leave()
            {
                (_Provider as Regulus.Remoting.Ghost.IProvider).Remove(_Id);
            }

            void Game.IStage.Update()
            {
                if (_Agent.Connected == false)
                    BreakEvent();
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
        }
    }
    partial class RemotingUser : Regulus.Project.SamebestKeys.IUser
    {
        class ConnectStage : Regulus.Game.IStage
        {
            Connect _Connecter;
            public event Action<bool> ResultEvent;
            private Regulus.Remoting.Ghost.TProvider<Project.SamebestKeys.IConnect> _ConnectProvider;
            private Regulus.Remoting.Ghost.Native.Agent _Agent;

            public ConnectStage(Regulus.Remoting.Ghost.TProvider<Project.SamebestKeys.IConnect> connect_provider, Regulus.Remoting.Ghost.Native.Agent agent)
            {
                // TODO: Complete member initialization
                this._ConnectProvider = connect_provider;
                this._Agent = agent;
                _Connecter = new Connect();
            }

            void Game.IStage.Enter()
            {
                _Connecter.ConnectedEvent += _OnConnect;
                (_ConnectProvider as Regulus.Remoting.Ghost.IProvider).Add(_Connecter);
                (_ConnectProvider as Regulus.Remoting.Ghost.IProvider).Ready(_Connecter.Id);
            }

            void _OnConnect(string ipaddr, int port, Regulus.Remoting.Value<bool> result)
            {
                var value = _Agent.Connect(ipaddr, port);
                value.OnValue += (ret) =>
                {
                    result.SetValue(ret);
                    ResultEvent(ret);
                };
            }
            void Game.IStage.Leave()
            {
                (_ConnectProvider as Regulus.Remoting.Ghost.IProvider).Remove(_Connecter.Id);
            }

            void Game.IStage.Update()
            {
                
            }
        }
    }
    partial class RemotingUser : Regulus.Project.SamebestKeys.IUser
    {
        Connect _Connecter;

        Regulus.Remoting.Ghost.TProvider<Regulus.Project.SamebestKeys.IConnect> _ConnectProvider;
        Regulus.Remoting.Ghost.TProvider<Regulus.Project.SamebestKeys.IOnline> _OnlineProvider;
        private Regulus.Remoting.Ghost.Native.Agent _Complex;
        private Regulus.Remoting.IAgent _Agent { get { return _Complex; } }

        Regulus.Game.StageMachine _Machine;
        Regulus.Utility.Updater _Updater;
        public RemotingUser()
        {
            _Machine = new Game.StageMachine();
            _Connecter = new Connect();
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
            var stage = new ConnectStage(_ConnectProvider, _Complex);
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
            var stage = new OnlineStage(_OnlineProvider, _Complex);

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
    }
}


