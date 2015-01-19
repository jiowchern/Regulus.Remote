using System;

namespace Regulus.Project.SamebestKeys
{
    class Actor : Entity
	{
		// Entity屬性
        Serializable.EntityPropertyInfomation _Property;
		// Entity外表資訊
        Serializable.EntityLookInfomation _Look;
		// 身體寬度
        float _BodyWidth;
		// 身體高度
        float _BodyHeight;

        Behavior _Behavior;

        public ActionStatue CurrentAction { get; protected set; }

        ActorPropertyAbility _PropertyAbility;

		public Actor(Serializable.EntityPropertyInfomation property , Serializable.EntityLookInfomation look)
            : base(property.Id)
		{
            _Property = property;
            _Look = look;
            _BodyWidth = 1;
            _BodyHeight = 1;

            _Behavior = new Behavior(this);
            _PropertyAbility = new ActorPropertyAbility(_Property, _Look);
		}

		/// <summary>
		/// 移動功能
		/// </summary>
        private ActorMoverAbility2 _MoverAbility;

        PhysicalAbility _QuadTreeObjectAbility;
        public Action<Serializable.MoveInfomation> ShowActionEvent;

		/// <summary>
		/// 設定功能
		/// </summary>
		/// <param name="abilitys">現有功能Dict</param>
        protected override void _SetAbility(Entity.AbilitySet abilitys)
        {
            _MoverAbility = new ActorMoverAbility2(_Property.Direction, _Property.Position.X, _Property.Position.Y);
            _MoverAbility.ActionEvent += _OnAction;
            _MoverAbility.PositionEvent += _OnPosition;
            
            abilitys.AttechAbility<IMoverAbility>(_MoverAbility);

            _QuadTreeObjectAbility = new PhysicalAbility(new Regulus.CustomType.Rect(_Property.Position.X - _BodyWidth / 2, _Property.Position.Y - _BodyHeight / 2, _BodyWidth, _BodyHeight), this);
            abilitys.AttechAbility<PhysicalAbility>(_QuadTreeObjectAbility);
            abilitys.AttechAbility<IBehaviorAbility>(_Behavior);
            abilitys.AttechAbility<IBehaviorCommandAbility>(_Behavior);

            abilitys.AttechAbility<IActorPropertyAbility>(_PropertyAbility);
            abilitys.AttechAbility<IActorUpdateAbility>(_PropertyAbility);
            abilitys.AttechAbility<ISkillCaptureAbility>(_PropertyAbility);
            
            
        }

		/// <summary>
		/// On位置改變
		/// </summary>
		/// <param name="time">目前時間Ticks</param>
		/// <param name="unit_vector">單位時間移動向量</param>
        private void _OnPosition(long time, CustomType.Vector2 unit_vector)
        {
            _Property.Position = CustomType.Vector2.FromPoint(unit_vector.X + _Property.Position.X, unit_vector.Y + _Property.Position.Y);            
            _MoverAbility.SetPosition(_Property.Position.X, _Property.Position.Y);
            _QuadTreeObjectAbility.UpdateBounds(_Property.Position.X - _BodyWidth / 2, _Property.Position.Y - _BodyHeight / 2);
        }

		/// <summary>
		/// 移除功能
		/// </summary>
		/// <param name="abilitys">現有功能Dict</param>
        protected override void _RiseAbility(Entity.AbilitySet abilitys)
        {
            abilitys.DetechAbility<IActorPropertyAbility>();
            abilitys.DetechAbility<IActorUpdateAbility>();
            abilitys.DetechAbility<ISkillCaptureAbility>();

            abilitys.DetechAbility<IBehaviorCommandAbility>();            
            abilitys.DetechAbility<IBehaviorAbility>();
            abilitys.DetechAbility<IMoverAbility>();
            abilitys.DetechAbility<PhysicalAbility>();
        }

		// 方向
        public float Direction { get { return _Property.Direction; } }

        void _OnAction(long begin_time, float move_speed,float direction, Regulus.CustomType.Vector2 unit_vector, ActionStatue action_state)
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
            mi.Mode = Mode;

            if (ShowActionEvent != null)
                ShowActionEvent(mi);
        }

		/// <summary>
		/// 設定位置
		/// </summary>
        public void SetPosition(float x, float y)
        {
            _Property.Position = CustomType.Vector2.FromPoint(x,y);            
            _MoverAbility.SetPosition(_Property.Position.X, _Property.Position.Y);
            _QuadTreeObjectAbility.UpdateBounds(_Property.Position.X - _BodyWidth / 2, _Property.Position.Y - _BodyHeight / 2);
        }

        public ActorMode Mode { get { return _PropertyAbility.GetMode(); } }
    }
}
