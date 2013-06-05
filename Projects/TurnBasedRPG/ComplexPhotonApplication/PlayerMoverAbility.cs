using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
	class ActorMoverAbility : IMoverAbility
	{
        Actor _Actor;
        ActionStatue _CurrentAction; 
		public ActorMoverAbility(Actor actor)
		{
            _Actor = actor;
            _Update = _Empty;
		}

        Action<long, CollisionInformation> _Update;
		void IMoverAbility.Update(long time, CollisionInformation collision_information)
		{
            _Update(time, collision_information);
		}
        long    _CurrentTime;
        float   _MoveSpeed;
        Regulus.Types.Vector2 _UnitVector;
        float _Direction;
        void IMoverAbility.Act(ActionStatue action_statue, float move_speed, float direction)
        {
            if (_CanSwitch(_CurrentAction, action_statue))
            {
                var t = (float)(direction * Math.PI / 180);
                _Direction = direction;
                _CurrentAction = action_statue;
                _MoveSpeed = move_speed;
                _UnitVector = new Types.Vector2() { X = (float)Math.Cos(t), Y = (float)Math.Sin(t) };
                _Update = _First;                 
            }
        }

        public Action<long, Regulus.Types.Vector2> PositionEvent;
        void _UpdateMover(long time, CollisionInformation collision_information)
        {
            if (_MoveSpeed > 0)
            {
                var dt = (float)new System.TimeSpan(time - _CurrentTime).TotalSeconds;

                Regulus.Types.Vector2 moveVector = new Types.Vector2();
                moveVector.X = _UnitVector.X * dt * _MoveSpeed; 
                moveVector.Y = _UnitVector.Y * dt * _MoveSpeed;

                _CurrentTime = time;

                if (PositionEvent != null)
                    PositionEvent(time, moveVector);
            }
            else
            {
                _Update = _Empty;
            }
            
        }

        public delegate void BeginAction(long begin_time, float speed, float direction , Regulus.Types.Vector2 vector , ActionStatue action_status);
        public BeginAction ActionEvent;

        void _Empty(long time, CollisionInformation collision_information)
        { 
        }
        void _First(long time, CollisionInformation collision_information)
        {
            if (ActionEvent != null)
                ActionEvent(time, _MoveSpeed, _Direction, _UnitVector, _CurrentAction);
            _Update = _UpdateMover;
            _CurrentTime = time;
        }

        private bool _CanSwitch(ActionStatue _CurrentAction, ActionStatue action_statue)
        {
            // TODO 動做兼是否可切換
            return true;
        }
    }
}
