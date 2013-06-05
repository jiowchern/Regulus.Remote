using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
	class Actor : Entity
	{
        Serializable.EntityPropertyInfomation _Property;
        Serializable.EntityLookInfomation _Look;
        
        public ActionStatue CurrentAction { get; protected set; }        

		public Actor(Serializable.EntityPropertyInfomation property , Serializable.EntityLookInfomation look)
            : base(property.Id)
		{
            _Property = property;
            _Look = look;
		}        

        private ActorMoverAbility _MoverAbility;
        public Action<Serializable.MoveInfomation> ShowActionEvent;

        protected override void _SetAbility(Entity.AbilitySet abilitys)
        {
            _MoverAbility = new ActorMoverAbility(this);
            _MoverAbility.ActionEvent += _OnAction;
            _MoverAbility.PositionEvent += _OnPosition;
            
            abilitys.AttechAbility<IMoverAbility>(_MoverAbility);
        }

        private void _OnPosition(long time, Types.Vector2 unit_vector)
        {
            _Property.Position.X += unit_vector.X;
            _Property.Position.Y += unit_vector.Y;
        }

        protected override void _RiseAbility(Entity.AbilitySet abilitys)
        {
            abilitys.DetechAbility<IMoverAbility>();
        }

        public float Direction {get ; private set ; }
        void _OnAction(long begin_time, float move_speed,float direction, Regulus.Types.Vector2 unit_vector, ActionStatue action_state)
        {
            Direction = direction;
            Serializable.MoveInfomation mi = new Serializable.MoveInfomation();
            mi.ActionStatue = action_state;
            mi.MoveDirectionAngle = direction;
            CurrentAction = action_state;

            mi.BeginPosition = _Property.Position;
            mi.BeginTime = begin_time;
            mi.MoveDirection = unit_vector;
            mi.Speed = move_speed;

            if (ShowActionEvent != null)
                ShowActionEvent(mi);
        }
        
    }
}
