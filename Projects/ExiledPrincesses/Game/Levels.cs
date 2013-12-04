using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Regulus.Project.ExiledPrincesses.Game
{
  
    public interface ILevels 
    {
        event Action<string> ToLevelsEvent;
        event Action<string> ToTownEvent;
        void Leave(Squad squad);
    }

    partial class Levels : ILevels, Regulus.Utility.IUpdatable
    {        

        float _Position;        
        Regulus.Game.StageMachine _StageMachine;
        Guid _Id;
        private MapPrototype _MapPrototype;
        public Guid Id { get { return _Id; } }
        System.Collections.Generic.Queue<Station> _Stations;
        public delegate void OnRelease();
        public event OnRelease ReleaseEvent;
        Platoon _Platoon;
        Regulus.Utility.Updater<Platoon> _Platoons;
        public Levels(MapPrototype map_prototype, Squad squad)
        {
            _Position = 0.0f;
            _Id = Guid.NewGuid();            
            this._MapPrototype = map_prototype;
            _Stations = new Queue<Station>(_MapPrototype.Stations);
            _StageMachine = new Regulus.Game.StageMachine();

            _Platoon = new Platoon(squad);
            _Platoon.EmptyEvent += () => { ReleaseEvent(); };

            _Platoons = new Utility.Updater<Platoon>(); ;
        }

        private void _ToIdle()
        {
            var stage = new IdleStage(_Platoon);
            
            stage.GoForwardEvent += _ToGoForward;            
            _StageMachine.Push(stage);
        }

        void _ToGoForward()
        {
            if (_Stations.Count > 0)
            {
                var stage = new GoForwardStage(_Position, _Stations.Dequeue(), _Platoon);
                stage.ArrivalEvent += _OnArrival;
                _StageMachine.Push(stage);
            }            
        }

        void _OnArrival(float position, Station station)
        {
            _Position = position;
            if (station.Kind == Station.KindType.Choice)
            {
                _ToChoice(station);
            }
            else if (station.Kind == Station.KindType.Combat)
            {
                _ToCombat(station);
            }            
        }

        private void _ToCombat(Station station)
        {
            var battlefield = BattlefieldResources.Instance.Find(station.Id);
            if (battlefield != null)
            {
                /*var stage = new CombatStage(battlefield, _Platoon);
                stage.ResultEvent += _CombatResult;
                _StageMachine.Push(stage);*/
            }
            else
            {
                _ToIdle();
            }
        }

        void _CombatResult(Levels.CombatStage.Result result)
        {
            if (result == CombatStage.Result.Victory)
            {
                _ToIdle();
            }
            else
            {
                _ToTone(_MapPrototype.Tone);
            }
        }

        private void _ToChoice(Station station)
        {
            var prototype = ChoiceResource.Instance.Find(station.Id);
            if (prototype != null)
            {
                var stage = new ChoiceStage(prototype, _Platoon);
                stage.ToMapEvent += _ToMap;
                stage.ToTownEvent += _ToTone;
                stage.CancelEvent += _ToIdle;
                _StageMachine.Push(stage);
            }
            else
            {
                throw new SystemException("沒有選項:"+station.Id);
            }
        }

        void _ToTone(string name)
        {            
            _ToToneEvent(name);
            ReleaseEvent();
        }

        void _ToMap(string name)
        {
            _ToMapEvent(name);
            ReleaseEvent();
        }

        event Action<string> _ToMapEvent;
        event Action<string> ILevels.ToLevelsEvent
        {
            add { _ToMapEvent += value; }
            remove { _ToMapEvent -= value; }
        }
        event Action<string> _ToToneEvent;
        event Action<string> ILevels.ToTownEvent
        {
            add { _ToToneEvent += value; }
            remove { _ToToneEvent -= value; }
        }

        bool Utility.IUpdatable.Update()
        {
            _Platoons.Update();
            _StageMachine.Update();
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            _Platoons.Add(_Platoon);
            _ToIdle();            
        }

        void Framework.ILaunched.Shutdown()
        {
            _StageMachine.Termination();
            _Platoons.Remove(_Platoon);
        }




        void ILevels.Leave(Squad squad)
        {
            _Platoon.Leave(squad);
        }
        
    }
    partial class Levels
    {
        class CombatStage : Regulus.Game.IStage
        {
            public enum Result
            {
                Victory,Failure
            }
            public delegate void OnResult(Result result);
            public event OnResult ResultEvent;
            
            private Contingent.FormationType _Formation;
            private ITeammate[] _Teammates;
            Combat _Combat;
            BattlefieldPrototype _Prototype;
            public CombatStage(BattlefieldPrototype prototype, Contingent.FormationType formation, ITeammate[] teammates)
            {
                _Prototype = prototype;
                this._Formation = formation;
                this._Teammates = teammates;

                _Combat = new Combat();
            }
            void Regulus.Game.IStage.Enter()
            {
                var team1 = new Team(_Formation, _Teammates);
                var enemys = (from enemy in _Prototype.Enemys select new Teammate( new ActorInfomation() { Exp = 0 , Prototype = enemy })).ToArray();
                var team2 = new Team(_Prototype.Formation, enemys);
                _Combat.Initial(team1, team2);
                _Combat.WinnerEvent += (winner) =>
                {
                    if (winner == team1)
                        ResultEvent(Result.Victory);
                    else
                        ResultEvent(Result.Failure);
                };

                _Combat.DrawEvent += () =>
                {
                    ResultEvent(Result.Failure);
                };
                
            }

            void Regulus.Game.IStage.Leave()
            {
                _Combat.Finial();
            }

            void Regulus.Game.IStage.Update()
            {
                _Combat.Update();
            }
        }
    }
    partial class Levels
    {
        class ChoiceStage : Regulus.Game.IStage, IAdventureChoice
        {
            public delegate void OnToTown(string name);
            public event OnToTown ToTownEvent;
            public delegate void OnToMap(string name);
            public event OnToMap ToMapEvent;
            public delegate void OnCancel();
            public event OnCancel CancelEvent;
            ChoicePrototype _ChoicePrototype;            
            Regulus.Standalong.Agent _Agent;
            
            ICommandable _Commandable; 
            public ChoiceStage(ChoicePrototype protorype, ICommandable commandable)
            {
                _ChoicePrototype = protorype;
                _Commandable = commandable;
                _Agent = new Standalong.Agent();
            }


            void _ChoiceMap(string name)
            {
                var result = (from map in _ChoicePrototype.Maps where name == map select map).Count();
                if (result >= 1)
                {
                    ToMapEvent(name);
                }
            }
            void _ChoiceTown(string name)
            {
                var result = (from town in _ChoicePrototype.Towns where name == town select town).Count();
                if (result >= 1)
                {
                    ToTownEvent(name);
                }
            }
            void _ChoiceCancel()
            {
                if (_ChoicePrototype.Cancel)
                {
                    CancelEvent();
                }
            }
            public void Enter()
            {
                _Agent.Launch();
                _Agent.Bind<IAdventureChoice>(this);
                _Commandable.AuthorizeChoice(_Agent.QueryProvider<IAdventureChoice>().Ghosts[0]);
            }

            public void Leave()
            {
                _Commandable.InterdictChoice(_Agent.QueryProvider<IAdventureChoice>().Ghosts[0]);                
                _Agent.Unbind<IAdventureChoice>(this);
                _Agent.Shutdown();
            }

            public void Update()
            {
                _Agent.Update();
            }

            string[] IAdventureChoice.Maps
            {
                get { return _ChoicePrototype.Maps; }
            }

            string[] IAdventureChoice.Town
            {
                get { return _ChoicePrototype.Towns; }
            }


            void IAdventureChoice.GoMap(string map)
            {
                _ChoiceMap(map);
            }

            void IAdventureChoice.GoTown(string tone)
            {
                _ChoiceTown(tone);
            }
        }
    }
    partial class Levels
    {
        class GoForwardStage : Regulus.Game.IStage , IAdventureGo
        {
            Station _Station;
            private float _Position;
            const float _DistancePerSeconds = 140.0f;

            public delegate void OnArrival(float position, Station station);
            public event OnArrival ArrivalEvent;
            
            Regulus.Standalong.Agent _Agent;
            Platoon _Platoon;
            
            public GoForwardStage(float position, Station station, Platoon platoon)
            {
                _Station = station;
                this._Position = position;
                _Platoon = platoon;
                
                _Agent = new Standalong.Agent();
                
            }

            void Regulus.Game.IStage.Enter()
            {
                _Agent.Launch();
                _Agent.Bind<IAdventureGo>(this);
                var ghost = _Agent.QueryProvider<IAdventureGo>().Ghosts[0];
                _Platoon.Go(ghost);

                _ForwardEvent(LocalTime.Instance.Ticks, _Position, _DistancePerSeconds);
                
            }

            void Regulus.Game.IStage.Leave()
            {
                
                _ForwardEvent(LocalTime.Instance.Ticks, _Position, 0);
                _Platoon.Stop();
                _Agent.Unbind<IAdventureGo>(this);
                
                _Agent.Shutdown();
            }

            void Regulus.Game.IStage.Update()
            {                
                _Agent.Update();
                _Position += (_DistancePerSeconds * LocalTime.Instance.DeltaSecond);
                if (_Position > _Station.Position)
                {
                    ArrivalEvent(_Position, _Station);
                }
            }

            event Action<long /*time_tick*/ , float /*position*/ , float /*speed*/> _ForwardEvent;
            event Action<long /*time_tick*/ , float /*position*/ , float /*speed*/> IAdventureGo.ForwardEvent
            {
                add { _ForwardEvent += value; }
                remove { _ForwardEvent -= value; }
            }
        }
    }

    partial class Levels
    {

        public class IdleStage : Regulus.Game.IStage, IAdventureIdle
        {
            public delegate void OnGoForward();
            public event OnGoForward GoForwardEvent;

            ICommandable _Commandable;
            Regulus.Standalong.Agent _Agent;
            IdleStage()
            {
                _Agent = new Standalong.Agent();
            }
            public IdleStage(ICommandable commandable)
                : this()
            {
                _Commandable = commandable;
            }
            void Regulus.Game.IStage.Enter()
            {
                _Agent.Launch();
                _Agent.Bind<IAdventureIdle>(this);
                _Commandable.AuthorizeIdle(Get());
                
            }
            public IAdventureIdle Get()
            {
                var provider = _Agent.QueryProvider<IAdventureIdle>();
                return provider.Ghosts[0];
            }            
            
            void _GoForwar()
            {
                if (GoForwardEvent != null)
                    GoForwardEvent();
                GoForwardEvent = null;
            }

            void Regulus.Game.IStage.Leave()
            {
                _Commandable.InterdictIdle(null);
                _Agent.Unbind<IAdventureIdle>(this);
                _Agent.Shutdown();
            }

            void Regulus.Game.IStage.Update()
            {
                _Agent.Update();
            }

            void IAdventureIdle.GoForwar()
            {
                _GoForwar();
            }
        }
    }
}
