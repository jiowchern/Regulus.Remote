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
        float _BodyWidth;
        float _BodyHeight;
        
        public ActionStatue CurrentAction { get; protected set; }        

		public Actor(Serializable.EntityPropertyInfomation property , Serializable.EntityLookInfomation look)
            : base(property.Id)
		{
            _Property = property;
            _Look = look;
            _BodyWidth = 1;
            _BodyHeight = 1;
		}

		

        private ActorMoverAbility _MoverAbility;

        PhysicalAbility _QuadTreeObjectAbility;
        public Action<Serializable.MoveInfomation> ShowActionEvent;

        protected override void _SetAbility(Entity.AbilitySet abilitys)
        {
            _MoverAbility = new ActorMoverAbility(_Property.Direction, _Property.Position.X, _Property.Position.Y);
            _MoverAbility.ActionEvent += _OnAction;
            _MoverAbility.PositionEvent += _OnPosition;
            
            abilitys.AttechAbility<IMoverAbility>(_MoverAbility);

            _QuadTreeObjectAbility = new PhysicalAbility(new Regulus.Types.Rect(_Property.Position.X - _BodyWidth / 2, _Property.Position.Y - _BodyHeight / 2, _BodyWidth, _BodyHeight), this);
            abilitys.AttechAbility<PhysicalAbility>(_QuadTreeObjectAbility);


            
        }

        private void _OnPosition(long time, Types.Vector2 unit_vector)
        {


            _Property.Position = Types.Vector2.FromPoint(_Property.Position.X + unit_vector.X, _Property.Position.Y + unit_vector.Y);
            
            _MoverAbility.SetPosition(_Property.Position.X, _Property.Position.Y);
            _QuadTreeObjectAbility.UpdateBounds(_Property.Position.X - _BodyWidth / 2, _Property.Position.Y - _BodyHeight / 2);
            
        }

        protected override void _RiseAbility(Entity.AbilitySet abilitys)
        {
			abilitys.DetechAbility<Regulus.Physics.IQuadObject>();
            abilitys.DetechAbility<PhysicalAbility>();
            
        }

        public float Direction { get { return _Property.Direction; } }
        void _OnAction(long begin_time, float move_speed,float direction, Regulus.Types.Vector2 unit_vector, ActionStatue action_state)
        {
            _Property.Direction = direction;
            
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

        public void SetPosition(float x, float y)
        {
            _Property.Position = Types.Vector2.FromPoint(x, y);            
            _MoverAbility.SetPosition(_Property.Position.X, _Property.Position.Y);
            _QuadTreeObjectAbility.UpdateBounds(_Property.Position.X - _BodyWidth / 2, _Property.Position.Y - _BodyHeight / 2);
        }
    }
}
