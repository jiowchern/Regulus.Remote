
using System;
namespace Regulus.Project.SamebestKeys
{

    using Regulus.Project.SamebestKeys.Serializable;
    using Regulus.Remoting;
    

    public interface IVerify
    {
        
        Value<bool> CreateAccount(string name, string password);
        Value<LoginResult> Login(string name, string password);
        void Quit();        
    };

    public interface IParking
    {
        Value<bool> CheckActorName(string name );
        Value<bool> CreateActor(EntityLookInfomation cai);
        Value<EntityLookInfomation[]> DestroyActor(string name);
        Value<EntityLookInfomation[]> QueryActors();
        void Back();
        Value<bool> Select(string name);
    }

	public interface IMapInfomation
	{
     
	}

    public interface IPlayer
    {
        Guid Id { get; }
        string Name { get; }
        float Speed { get; }
        float Direction { get; }		
        
        void Ready();
        void Logout();
        void ExitWorld();        
        void SetPosition(float x,float y);		
        void SetVision(int vision);

        void SetSpeed(float speed);
        void Walk(float direction);
        void Stop(float direction);
        void Say(string message);
		
        void BodyMovements(ActionStatue action_statue);
        Value<string> QueryMap();
        
    }



    public interface IObservedAbility 
    {
        string Name { get; }
        Guid Id { get; }
        Regulus.Types.Vector2 Position { get; }
        float Direction { get; }        
        event Action<MoveInfomation> ShowActionEvent;
        event Action<string> SayEvent;
    }

	public interface IMoverAbility
	{
        Regulus.Utility.OBB Obb { get; }
		
        void Act(ActionStatue action_statue, float move_speed, float direction);

        void Update(long time, System.Collections.Generic.IEnumerable<Utility.OBB> obbs);
    }
    
}


namespace Regulus.Project.SamebestKeys
{
    public class ActorMoverAbility : IMoverAbility
    {
        Utility.OBB _Obb;
        ActionStatue _CurrentAction;
        public ActorMoverAbility(float direction , float x , float y)
        {
            _Update = _Empty;
            _Direction = direction;
            _Obb = new Utility.OBB(x,y,0.5f,0.5f);
            
        }
        public void SetPosition(float x , float y)
        {
            _Obb.setXY(x,y);
        }
        Action<long, System.Collections.Generic.IEnumerable<Utility.OBB> > _Update;
        void IMoverAbility.Update(long time, System.Collections.Generic.IEnumerable<Utility.OBB> obbs)
        {
            _Update(time, obbs);
        }
        long _CurrentTime;
        float _MoveSpeed;
        Regulus.Types.Vector2 _UnitVector;
        float _Direction;

        void _Act(ActionStatue action_statue, float move_speed, float direction /* 轉向角度 0~360 */)
        {
            if (_CanSwitch(_CurrentAction, action_statue))
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
        }
        void IMoverAbility.Act(ActionStatue action_statue, float move_speed, float direction /* 轉向角度 0~360 */)
        {
            _Act(action_statue, move_speed, direction);
        }

        public event Action<long, Regulus.Types.Vector2> PositionEvent;
        void _UpdateMover(long time, System.Collections.Generic.IEnumerable<Utility.OBB> obbs)
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

                    Utility.OBB testobb = new Utility.OBB(_Obb.getX() + moveVector.X, _Obb.getY() + moveVector.Y, _Obb.getWidth(), _Obb.getHeight());
                    testobb.setRotation(_Obb.getRotation());
                    foreach (var obb in obbs)
                    {
                        if (testobb.isCollision(obb) == false)
                        {
                            continue;
                        }

                        Utility.OBB safeobb = new Utility.OBB(_Obb.getX() , _Obb.getY() , _Obb.getWidth(), _Obb.getHeight());
                        safeobb.setRotation(_Obb.getRotation());
                        do
                        {                            
                            moveVector.X += 0 - _UnitVector.X * dt * _MoveSpeed;
                            moveVector.Y += 0 - _UnitVector.Y * dt * _MoveSpeed;
                            safeobb.setXY(_Obb.getX() + moveVector.X, _Obb.getY() + moveVector.Y );
                        }
                        while (safeobb.isCollision(obb));
                        
                        if (PositionEvent != null)
                            PositionEvent(time, moveVector);

                        _Act(ActionStatue.GangnamStyle, 0, 0);
                        return;
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

        public delegate void BeginAction(long begin_time, float speed, float direction, Regulus.Types.Vector2 vector, ActionStatue action_status);
        public event BeginAction ActionEvent;

        void _Empty(long time, System.Collections.Generic.IEnumerable<Utility.OBB> obbs)
        {
        }
        void _First(long time, System.Collections.Generic.IEnumerable<Utility.OBB> obbs)
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

        Utility.OBB IMoverAbility.Obb
        {
            get { return _Obb; }
        }
    }
}
