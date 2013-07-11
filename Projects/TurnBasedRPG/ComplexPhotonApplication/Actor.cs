using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
	class Actor : Entity ,Regulus.Physics.IQuadObject
	{
        Serializable.EntityPropertyInfomation _Property;
        Serializable.EntityLookInfomation _Look;
        
        public ActionStatue CurrentAction { get; protected set; }        

		public Actor(Serializable.EntityPropertyInfomation property , Serializable.EntityLookInfomation look)
            : base(property.Id)
		{
            _Property = property;
            _Look = look;
			_Bounds = new System.Windows.Rect( new System.Windows.Size(1 , 1));

			_UpdateBounds(_Property.Position , _Bounds);
		}

		private void _UpdateBounds(Types.Vector2 vector2, System.Windows.Rect bounds)
		{
			bounds.Offset(vector2.X, vector2.Y);
			if (BoundsChanged != null)
				BoundsChanged(bounds , new EventArgs());
		}        

        private ActorMoverAbility _MoverAbility;
        public Action<Serializable.MoveInfomation> ShowActionEvent;

        protected override void _SetAbility(Entity.AbilitySet abilitys)
        {
            _MoverAbility = new ActorMoverAbility(_Property.Direction);
            _MoverAbility.ActionEvent += _OnAction;
            _MoverAbility.PositionEvent += _OnPosition;
            
            abilitys.AttechAbility<IMoverAbility>(_MoverAbility);
			abilitys.AttechAbility<Regulus.Physics.IQuadObject>(this);
        }

        private void _OnPosition(long time, Types.Vector2 unit_vector)
        {
            _Property.Position.X += unit_vector.X;
            _Property.Position.Y += unit_vector.Y;

			_UpdateBounds(unit_vector , _Bounds);
        }

        protected override void _RiseAbility(Entity.AbilitySet abilitys)
        {
			abilitys.DetechAbility<Regulus.Physics.IQuadObject>();
            abilitys.DetechAbility<IMoverAbility>();
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

		System.Windows.Rect _Bounds;
		System.Windows.Rect Physics.IQuadObject.Bounds
		{
			get { return _Bounds; }
		}

		public event EventHandler BoundsChanged;
		
	}
}
