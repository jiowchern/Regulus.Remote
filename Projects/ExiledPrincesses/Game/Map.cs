using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.ExiledPrincesses.Game
{
    

    public interface IMap 
    {
        event Action<string> ToMapEvent;
        event Action<string> ToToneEvent;

        Guid Id { get; }
    }

    partial class Map : IMap, Regulus.Utility.IUpdatable
    {        

        float _Position;
        
        Regulus.Game.StageMachine _StageMachine;
        Guid _Id;
        private MapPrototype _MapPrototype;
        public Guid Id { get { return _Id; } }
        System.Collections.Generic.Queue<Station> _Stations;
        Contingent.FormationType _Formation;
        ITeammate[] _Teammates;
        public Map(MapPrototype map_prototype)
        {
            _Position = 0.0f;
            _Id = Guid.NewGuid();            
            this._MapPrototype = map_prototype;
            _Stations = new Queue<Station>(_MapPrototype.Stations);
        }

        internal void Initialize(Contingent.FormationType formation, ITeammate[] teammate)
        {
            _Formation = formation;
            _Teammates = teammate;
            _ToIdle();
        }

        internal void Release()
        {
            
        }

        private void _ToIdle()
        {
            var stage = new IdleStage();
            stage.GoForwardEvent += _ToGoForward;
            _StageMachine.Push(stage);
        }

        void _ToGoForward()
        {
            var stage = new GoForwardStage(_Position, _Stations.Dequeue());
            stage.ArrivalEvent += _OnArrival;
            _StageMachine.Push(stage);
        }

        void _OnArrival(float position, Station station)
        {
            _Position = position;
            if (station.GetKind() == Station.Kind.Choice)
            {
                _ToChoice(station);
            }
            else if (station.GetKind() == Station.Kind.Combat)
            {
                _ToCombat(station);
            }            
        }

        private void _ToCombat(Station station)
        {
            var battlefield = BattlefieldResources.Instance.Find(station.Id);
            if (battlefield != null)
            {
                var stage = new CombatStage(battlefield, _Formation, _Teammates);
                stage.ResultEvent += _CombatResult;
                _StageMachine.Push(stage);
            }
            else
            {
                _ToIdle();
            }
        }

        void _CombatResult(Map.CombatStage.Result result)
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
            var stage = new ChoiceStage(station.Id);
            stage.ToMapEvent += _ToMap;
            stage.ToTownEvent += _ToTone;
            stage.CancelEvent += _ToIdle;
            _StageMachine.Push(stage);
        }

        void _ToTone(string name)
        {
            _ToToneEvent(name);
        }

        void _ToMap(string name)
        {
            _ToMapEvent(name);
        }

        

        event Action<string> _ToMapEvent;
        event Action<string> IMap.ToMapEvent
        {
            add { _ToMapEvent += value; }
            remove { _ToMapEvent -= value; }
        }
        event Action<string> _ToToneEvent;
        event Action<string> IMap.ToToneEvent
        {
            add { _ToToneEvent += value; }
            remove { _ToToneEvent -= value; }
        }

        


        bool Utility.IUpdatable.Update()
        {
            _StageMachine.Update();
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            _Stations = new Queue<Station>();
            _StageMachine = new Regulus.Game.StageMachine();            
        }

        void Framework.ILaunched.Shutdown()
        {
            _StageMachine.Termination();
        }

        
    }
    partial class Map
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
    partial class Map
    {
        class ChoiceStage : Regulus.Game.IStage
        {
            public delegate void OnToTown(string name);
            public event OnToTown ToTownEvent;
            public delegate void OnToMap(string name);
            public event OnToMap ToMapEvent;
            public delegate void OnCancel();
            public event OnCancel CancelEvent;

            public ChoiceStage(Guid id)
            { 

            }
            public void Enter()
            {
                
            }

            public void Leave()
            {
                
            }

            public void Update()
            {
                
            }
        }
    }
    partial class Map
    {

        class GoForwardStage : Regulus.Game.IStage
        {
            Station _Station;
            private float _Position;
            const float _DistancePerSeconds = 10.0f;

            public delegate void OnArrival(float position, Station station);
            public event OnArrival ArrivalEvent;
            public GoForwardStage(float position , Station station)
            {
                _Station = station;
                this._Position = position;
            }

            void Regulus.Game.IStage.Enter()
            {
                
            }

            void Regulus.Game.IStage.Leave()
            {
                
            }

            void Regulus.Game.IStage.Update()
            {
                _Position += (_DistancePerSeconds * LocalTime.Instance.DeltaSecond);
                if (_Position > _Station.Position)
                {
                    ArrivalEvent(_Position, _Station);
                }
            }
        }
    }

    partial class Map
    {
        class IdleStage : Regulus.Game.IStage
        {
            public delegate void OnGoForward();
            public event OnGoForward GoForwardEvent;
            

            public IdleStage()
            {
                
            }
            void Regulus.Game.IStage.Enter()
            {
                
            }

            void _OnGoForwar()
            {
                if (GoForwardEvent != null)
                    GoForwardEvent();
            }

            void Regulus.Game.IStage.Leave()
            {                
            }

            void Regulus.Game.IStage.Update()
            {

            }
        }
    }
}
