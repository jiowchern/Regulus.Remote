
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    interface IActorUpdateAbility
    {
        void Update();
    }
    interface IActorPropertyAbility
    {
        void Injury(int damage);

        float Direction { get; }

        float NormalSpeed { get; }

        float CurrentSpeed { get; }

        int Hp { get; }

        bool IsAlive();

        void DoWakin();

        bool HasSkill(int skill);

        Remoting.Value<CastResult> Cast(int skill);

        void BreakCast();

        CastStep QueryCastStep(int skill);


        void ChangeMode();
    }

    class ActorPropertyAbility : IActorPropertyAbility, IActorUpdateAbility , ISkillCaptureAbility
    {
        ActorMode _Mode;
        
        Regulus.Utility.Updater _Updater;
        // Entity屬性
        Serializable.EntityPropertyInfomation _Property;
        // Entity外表資訊
        Serializable.EntityLookInfomation _Look;

        ActorCast _CurrentActorCast;

        public ActorPropertyAbility(Serializable.EntityPropertyInfomation property , Serializable.EntityLookInfomation look)
        {
            _Property = property;
            _Look = look;

            _Updater = new Utility.Updater();
        }

        void IActorPropertyAbility.Injury(int damage)
        {
            _Property.Health -= damage;
            _Property.Died = true;
        }

        float IActorPropertyAbility.Direction
        {
            get { return _Property.Direction; }
        }

        float IActorPropertyAbility.NormalSpeed
        {
            get { return _Property.Speed; }
        }

        float IActorPropertyAbility.CurrentSpeed
        {
            get { return _Property.Speed; }
        }

        int IActorPropertyAbility.Hp
        {
            get { return _Property.Health; }
        }

        bool IActorPropertyAbility.IsAlive()
        {
            return _Property.Died == false;
        }

        void IActorPropertyAbility.DoWakin()
        {
            _Updater.Add(new ActorDoWakin(_Property));
        }


        
        bool IActorPropertyAbility.HasSkill(int skill)
        {
            return _FindSkill(skill) != null;
        }

        private Regulus.Project.SamebestKeys.Serializable.Skill _FindSkill(int skill)
        {
            return (from s in _Property.Skills where s.Id == skill select s).SingleOrDefault();
        }

        Remoting.Value<CastResult> IActorPropertyAbility.Cast(int id)
        {
            Remoting.Value<CastResult> returnValue = new Remoting.Value<CastResult>();
            var skill = _FindSkill(id);
            if (skill != null)
            {
                _CurrentActorCast = new ActorCast(skill, this, returnValue);                
                _Updater.Add(_CurrentActorCast);
            }
            else
            {
                returnValue.SetValue(CastResult.Miss);
            }

            return returnValue;
        }

        CastStep IActorPropertyAbility.QueryCastStep(int skill)
        {
            if (_CurrentActorCast != null && skill == _CurrentActorCast.Skill)
                return _CurrentActorCast.Step;
            return CastStep.NotYet;
        }

        void IActorUpdateAbility.Update()
        {
            _Updater.Update();
        }


        void IActorPropertyAbility.BreakCast()
        {
            if (_CurrentActorCast != null)
                _CurrentActorCast.Break();
        }

        bool _InEffect;
        Types.Rect _Capture;
        internal void ClearCaptureRect()
        {
            _InEffect = false;
        }

        internal void SetCaptureRect(Types.Rect rect)
        {
            _InEffect = true;
            
            _Capture = rect;            
        }

        

       

        bool ISkillCaptureAbility.TryGetBounds(ref Types.Rect bounds , ref int skill)
        {            
            if (_InEffect)
            {
                skill = _CurrentActorCast.Skill;
                bounds = _Capture;
                bounds.Location = new Types.Point(_Capture.Location.X + _Property.Position.X, _Capture.Location.Y + _Property.Position.Y);                
            }
            return _InEffect;
        }


        void ISkillCaptureAbility.Hit()
        {
            
            _CurrentActorCast.Hit();
            
        }


        void IActorPropertyAbility.ChangeMode()
        {
            if (_Mode == ActorMode.Alert)
                _Mode = ActorMode.Explore;
            else
                _Mode = ActorMode.Alert;
        }
        public ActorMode GetMode() { return _Mode; }
    }
    
}
