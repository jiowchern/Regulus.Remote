using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    interface IContingent
    {
        Guid Id { get; set; }
        event Action GoForwardEvent; 
    }

    partial class Map : IMap
    {
        class Station
        {
            public Guid Id;
            public enum Kind
            {
                None,
                Combat,
                Choice
            }
            virtual public Kind GetKind()
            { 
                return Kind.None;
            }
            public float Position { get; set; }
        }

        float _Position;
        IContingent _Contingent;
        Regulus.Game.StageMachine _StageMachine;
        string _Tone;

        System.Collections.Generic.Queue<Station> _Stations;
        public Map()
        {
            _Position = 0.0f;            
        }

        bool Regulus.Game.IFramework.Update()
        {
            _StageMachine.Update();
            return true;
        }

        void Regulus.Game.IFramework.Launch()
        {
            _Stations = new Queue<Station>();
            _StageMachine = new Regulus.Game.StageMachine();

            _ToIdle();
        }

        private void _ToIdle()
        {
            var stage = new IdleStage(_Contingent);
            stage.GoForwardEvent += _ToGoForward;
            _StageMachine.Push(stage);
        }

        void _ToGoForward()
        {
            var stage = new GoForwardStage(_Position, _Stations.Dequeue());
            stage.ArrivalEvent += _OnArrival;
            _StageMachine.Push(stage);
        }

        void _OnArrival(float position, Map.Station station)
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
            var stage = new CombatStage(station.Id, _Formation, _Teammates);
            stage.ResultEvent += _CombatResult;
            _StageMachine.Push(stage);
        }

        void _CombatResult(Map.CombatStage.Result result)
        {
            if (result == CombatStage.Result.Victory)
            {
                _ToIdle();
            }
            else
            {
                _ToTone(_Tone);
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

        void Regulus.Game.IFramework.Shutdown()
        {
            _StageMachine.Termination();
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

        Contingent.FormationType _Formation;
        ITeammate[] _Teammates;
        void IMap.Initial(Contingent.FormationType formation, ITeammate[] teammate)
        {
            _Formation = formation;
            _Teammates = teammate;
        }

        void IMap.Release()
        {
            
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
            private Guid _Id;            
            private Contingent.FormationType _Formation;
            private ITeammate[] _Teammates;
            Combat _Combat;
            public CombatStage(Guid guid, Contingent.FormationType formation, ITeammate[] teammates)
            {
                // TODO: Complete member initialization
                this._Id = guid;
                this._Formation = formation;
                this._Teammates = teammates;

                _Combat = new Combat();
            }
            void Regulus.Game.IStage.Enter()
            {
                var team1 = new Combat.Team(_Formation, _Teammates);
                var team2 = new Combat.Team(Contingent.FormationType.Auxiliary , new ITeammate[]{ new Monster()});
                _Combat.Initial(team1, team2);
                
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
            private IContingent _Contingent;

            public IdleStage(IContingent contingent)
            {
                this._Contingent = contingent;
            }
            void Regulus.Game.IStage.Enter()
            {
                _Contingent.GoForwardEvent += _OnGoForwar;
            }

            void _OnGoForwar()
            {
                if (GoForwardEvent != null)
                    GoForwardEvent();
            }

            void Regulus.Game.IStage.Leave()
            {
                _Contingent.GoForwardEvent -= _OnGoForwar;
            }

            void Regulus.Game.IStage.Update()
            {

            }
        }
    }
}
