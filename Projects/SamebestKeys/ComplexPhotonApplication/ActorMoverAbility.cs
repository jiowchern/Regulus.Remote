using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Regulus.Project.SamebestKeys
{
	/// <summary>
	/// 其他玩家移動功能
	/// </summary>
    public class ActorMoverAbility2 : IMoverAbility
    {
        Regulus.Types.Polygon _Polygon;
        private float _Direction;
        Action<long, System.Collections.Generic.IEnumerable<Types.Polygon>> _Update;
        private ActionStatue _CurrentAction;
        private float _MoveSpeed;
        private Types.Vector2 _UnitVector;
        private long _PrevTime;
        public delegate void BeginAction(long begin_time, float speed, float direction, Regulus.Types.Vector2 vector, ActionStatue action_status);
        public event BeginAction ActionEvent;
        public event Action<long, Regulus.Types.Vector2> PositionEvent;

        public ActorMoverAbility2(float direction, float x, float y)
        {
            _Direction = direction;
            _Polygon = new Types.Polygon();
            _Polygon.Points.Add(new Types.Vector2(x - 0.25f, y + 0.25f));
            _Polygon.Points.Add(new Types.Vector2(x + 0.25f, y + 0.25f));
            _Polygon.Points.Add(new Types.Vector2(x + 0.25f, y - 0.25f));
            _Polygon.Points.Add(new Types.Vector2(x - 0.25f, y - 0.25f));
            _Polygon.BuildEdges();
            _Update = _Empty;

            _PrevTime = LocalTime.Instance.Ticks;
        }
        public void SetPosition(float x, float y)
        {
            var offset = Types.Vector2.FromPoint(x, y) - _Polygon.Center;
            _Polygon.Offset(offset);
        }
        Types.Polygon IMoverAbility.Polygon
        {
            get { return _Polygon; }
        }

        private void _Act(Serializable.ActionCommand action_command)        
        {
            // 角色面對世界的方向
            var moveDirection = 0.0f;
            if (action_command.Absolutely == false)
                moveDirection = (action_command.Direction + _Direction) % 360;
            else
                moveDirection = action_command.Direction % 360;

            if (action_command.Turn)
            {
                _Direction = moveDirection;
            }

            _CurrentAction = action_command.Command;
            _MoveSpeed = action_command.Speed;

            // 移動向量            
            var t = (float)((moveDirection - 180) * Math.PI / 180);
            var unitVector = new Types.Vector2() { X = -(float)Math.Sin(t), Y = -(float)Math.Cos(t) };
            
            _UnitVector = unitVector;
            _Update = _First;
            
            if (_PrevTime < action_command.Time)
                _PrevTime = action_command.Time;
        }
        

        void IMoverAbility.Update(long time, System.Collections.Generic.IEnumerable<Types.Polygon> polygons)
        {
            _Update(time , polygons);
        }

        void _First(long time, System.Collections.Generic.IEnumerable<Types.Polygon> obbs)
        {
            if (ActionEvent != null)
                ActionEvent(_PrevTime, _MoveSpeed, _Direction, _UnitVector, _CurrentAction);
            _Update = _UpdateMover;            
        }

        void _UpdateMover(long time, System.Collections.Generic.IEnumerable<Types.Polygon> polygons)
        {
            if (_MoveSpeed > 0)
            {
                var dt = (float)new System.TimeSpan(time - _PrevTime).TotalSeconds;
                if (dt > 0)
                {
                    Regulus.Types.Vector2 moveVector = new Types.Vector2();
                    moveVector.X = _UnitVector.X * dt * _MoveSpeed;
                    moveVector.Y = _UnitVector.Y * dt * _MoveSpeed;

                    _PrevTime = time;

                    var result = _Collision(polygons, moveVector);

                    if (PositionEvent != null && result.Intersect )
                        PositionEvent(time, moveVector + result.MinimumTranslationVector2);
                    else if (PositionEvent != null && result.WillIntersect )
                    {
                        //PositionEvent(time, moveVector + result.MinimumTranslationVector2);
                    }
                    else if (PositionEvent != null)
                        PositionEvent(time, moveVector);

                    if (result.Intersect || result.WillIntersect)
                    {

                        Serializable.ActionCommand actionCommand = new Serializable.ActionCommand() 
                        {
                            Absolutely = false,
                            Command = ActionStatue.Idle_1,
                            Direction = 0,
                            Speed = 0,
                            Time = time,
                            Turn = true
                        };
                        _Act(actionCommand);
                    }
                    
                }


            }
            else
            {
                _Update = _Empty;
            }

        }

        private Regulus.Types.Polygon.CollisionResult _Collision(System.Collections.Generic.IEnumerable<Types.Polygon> polygons, Regulus.Types.Vector2 moveVector)
        {
            var p = _Polygon;
            //Regulus.Types.Polygon p = Regulus.Utility.ValueHelper.DeepCopy<Regulus.Types.Polygon>(_Polygon);
            //p.Offset(moveVector);
            /*foreach(var point in _Polygon.Points )
            {
                p.Points.Add(point);
            }
            p.Convex();*/
            //p.BuildEdges();
            

            Regulus.Types.Polygon.CollisionResult result = new Types.Polygon.CollisionResult();
            foreach (var polygon in polygons)
            {
                result = Regulus.Types.Polygon.Collision(p, polygon, moveVector);
                if (result.WillIntersect || result.Intersect)
                {
                    return result;                    
                }
                
            }
            return result;
        }

        private void _Empty(long arg1, IEnumerable<Types.Polygon> arg2)
        {
            
        }


        void IMoverAbility.Act(Serializable.ActionCommand action_command)
        {
            _Act(action_command);
            
        }

        
    }
}
