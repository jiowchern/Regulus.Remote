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
        Regulus.Utility.StageMachine _StageMachine;
        Guid _Id;
        private MapPrototype _MapPrototype;
        public Guid Id { get { return _Id; } }
        System.Collections.Generic.Queue<Station> _Stations;
        public delegate void OnRelease();
        public event OnRelease ReleaseEvent;
        Platoon _Platoon;
        Regulus.Utility.Updater _Platoons;
        public Levels(MapPrototype map_prototype, Squad squad)
        {
            _Position = 0.0f;
            _Id = Guid.NewGuid();            
            this._MapPrototype = map_prototype;
            _Stations = new Queue<Station>(_MapPrototype.Stations);
            _StageMachine = new Regulus.Utility.StageMachine();

            _Platoon = new Platoon(squad);
            _Platoon.EmptyEvent += () => { ReleaseEvent(); };

            _Platoons = new Utility.Updater(); ;
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
                var stage = new CombatStage(battlefield, _Platoon);
                stage.ResultEvent += _CombatResult;
                _StageMachine.Push(stage);
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
    
    
    

    
}
