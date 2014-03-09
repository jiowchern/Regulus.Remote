﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Regulus.Project.SamebestKeys
{
    public class ActorMoverAbility2 : IMoverAbility2
    {
        Regulus.Types.Polygon _Polygon;
        private float _Direction;
        Action<long, System.Collections.Generic.IEnumerable<Types.Polygon>> _Update;
        private ActionStatue _CurrentAction;
        private float _MoveSpeed;
        private Types.Vector2 _UnitVector;
        private long _CurrentTime;
        public delegate void BeginAction(long begin_time, float speed, float direction, Regulus.Types.Vector2 vector, ActionStatue action_status);
        public event BeginAction ActionEvent;
        public event Action<long, Regulus.Types.Vector2> PositionEvent;

        public ActorMoverAbility2(float direction, float x, float y)
        {
            _Direction = direction;
            _Polygon = new Types.Polygon();
            _Polygon.Points.Add(new Types.Vector2(x - 0.5f, y + 0.5f));
            _Polygon.Points.Add(new Types.Vector2(x + 0.5f, y + 0.5f));
            _Polygon.Points.Add(new Types.Vector2(x + 0.5f, y - 0.5f));
            _Polygon.Points.Add(new Types.Vector2(x - 0.5f, y - 0.5f));
            _Polygon.BuildEdges();
        }
        public void SetPosition(float x, float y)
        {
            var offset = Types.Vector2.FromPoint(x, y) - _Polygon.Center;
            _Polygon.Offset(offset);
        }
        Types.Polygon IMoverAbility2.Polygon
        {
            get { return _Polygon; }
        }
        void _Act(ActionStatue action_statue, float move_speed, float direction /* 轉向角度 0~360 */)
        {
            // 角色面對世界的方向
            _Direction = (direction + _Direction) % 360;

            _CurrentAction = action_statue;
            _MoveSpeed = move_speed;

            var t = (float)((_Direction - 180) * Math.PI / 180);

            // 移動向量
            //_UnitVector = new Types.Vector2() { X = (float)Math.Cos(t), Y = (float)Math.Sin(t) };
            _UnitVector = new Types.Vector2() { X = -(float)Math.Sin(t), Y = -(float)Math.Cos(t) };
            _Update = _First;
        }

        void IMoverAbility2.Act(ActionStatue action_statue, float move_speed, float direction)
        {
            _Act(action_statue, move_speed, direction);
        }

        void IMoverAbility2.Update(long time, System.Collections.Generic.IEnumerable<Types.Polygon> polygons)
        {
            _Update(time , polygons);
        }

        void _First(long time, System.Collections.Generic.IEnumerable<Types.Polygon> obbs)
        {
            if (ActionEvent != null)
                ActionEvent(time, _MoveSpeed, _Direction, _UnitVector, _CurrentAction);
            _Update = _UpdateMover;
            _CurrentTime = time;
        }

        void _UpdateMover(long time, System.Collections.Generic.IEnumerable<Types.Polygon> polygons)
        {
            if (_MoveSpeed > 0)
            {
                var dt = (float)new System.TimeSpan(time - _CurrentTime).TotalSeconds;
                if (dt > 0)
                {
                    Regulus.Types.Vector2 moveVector = new Types.Vector2();
                    moveVector.X = _UnitVector.X * dt * _MoveSpeed;
                    moveVector.Y = _UnitVector.Y * dt * _MoveSpeed;

                    _CurrentTime = time;

                    foreach(var polygon in polygons)
                    {
                        var result = Regulus.Types.Polygon.Collision(_Polygon, polygon, moveVector);
                        if (result.WillIntersect)
                        {
                            _Act(ActionStatue.GangnamStyle, 0, 0);
                            return ;
                        }
                    }
                    if (PositionEvent != null)
                        PositionEvent(time, moveVector);
                }


            }
            else
            {
                _Update = _Empty;
            }

        }

        private void _Empty(long arg1, IEnumerable<Types.Polygon> arg2)
        {
            
        }
    }
}
