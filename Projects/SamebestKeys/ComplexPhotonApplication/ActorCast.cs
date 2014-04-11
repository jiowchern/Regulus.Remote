using Regulus.Project.SamebestKeys.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    partial class ActorCast : Regulus.Utility.IUpdatable
    {
        private Serializable.Skill _Skill;        
        private Remoting.Value<CastResult> _ReturnValue;
        Regulus.Game.StageMachine _Machine;


        bool _Done;
        CastStep _Step;
        Skill _Prototype;
        public CastStep Step { get { return _Step; } }

        public int Skill { get { return _Skill.Id; } }
        ActorPropertyAbility _Actor;
        public ActorCast(Serializable.Skill skill, ActorPropertyAbility actor, Remoting.Value<CastResult> return_value)
        {
            _Actor = actor;
            this._Skill = skill;
            
            this._ReturnValue = return_value;
            _Done = false;
            _Machine = new Game.StageMachine();
            _Step = CastStep.NotYet;
        }

        bool Utility.IUpdatable.Update()
        {
            _Machine.Update();
            return _Done == false;
        }

        void Framework.ILaunched.Launch()
        {
            Skill skillPrototype = GameData.Instance.FindSkill(_Skill.Id);
            _Prototype = skillPrototype;
            if (skillPrototype != null )
            {
                
                _ToBegin(skillPrototype.Begin);                
            }
            else
            {
                _Done = true;
            }

        }

        private void _ToBegin(float begin)
        {
            _Step = CastStep.Beginning;
            var stage = new After(begin);
            stage.DoneEvent += () => { _ToEffective(_Prototype.Effective); };
            _Machine.Push(stage);
        }

        private void _ToEffective(float effective)
        {
            _Step = CastStep.Effective;

            if (_Prototype.Capture)
            {
                var stage = new Effective(effective, _Actor, _Prototype.CaptureBounds);
                stage.DoneEvent += () =>
                {
                    _ReturnValue.SetValue(CastResult.Miss);
                    _ToEnd(_Prototype.End);
                };
                _Machine.Push(stage);
            }
            else
            { 

            }
            
        }

        private void _ToEnd(float end)
        {
            _Step = CastStep.Ending;
            var stage = new After(end);
            stage.DoneEvent += _OnDone;
            _Machine.Push(stage);
        }

        void Framework.ILaunched.Shutdown()
        {
            _Step = CastStep.NotYet;
            _Machine.Termination();            
        }
        void _OnDone()
        {            
            _Done = true;
        }
        internal void Break()
        {
            if (_Step != CastStep.Effective)
            {
                _ReturnValue.SetValue(CastResult.Break);
                _Done = true;
            }            
        }


        internal void Hit()
        {
            if (Step == CastStep.Effective)
            {
                _ReturnValue.SetValue(CastResult.Hit);
                _ToEnd(_Prototype.End);
            }            
        }
    }

    partial class ActorCast
    {
        class Effective : Regulus.Game.IStage
        {
            public delegate void OnDone();
            public event OnDone DoneEvent;
            private float _Interval;
            Regulus.Utility.TimeCounter _Timer;
            ActorPropertyAbility _Actor;
            Regulus.Types.Rect _Bounds;
            Effective(float interval)
            {                
                this._Interval = interval;
                _Timer = new Utility.TimeCounter();
            }

            public Effective(float interval,  ActorPropertyAbility actor , Regulus.Types.Rect bounds)
                : this(interval)
            {
                _Actor = actor;
                _Bounds = bounds;
            }

            void Game.IStage.Enter()
            {
                _Actor.SetCaptureRect(_Bounds);    
            }

            void Game.IStage.Leave()
            {
                _Actor.ClearCaptureRect();    
            }

            void Game.IStage.Update()
            {
                if (_Timer.Second > _Interval)
                    DoneEvent();
            }
        }

        class After : Regulus.Game.IStage
        {
            public delegate void OnDone();
            public event OnDone DoneEvent;
            private float _Interval;
            Regulus.Utility.TimeCounter _TimeCounter;
            public After(float interval)
            {
                // TODO: Complete member initialization
                this._Interval = interval;
                _TimeCounter = new Utility.TimeCounter();
            }

            void Game.IStage.Enter()
            {
                
            }

            void Game.IStage.Leave()
            {
                
            }

            void Game.IStage.Update()
            {
                if (_TimeCounter.Second > _Interval)
                    DoneEvent();
            }
        }
    }
}
