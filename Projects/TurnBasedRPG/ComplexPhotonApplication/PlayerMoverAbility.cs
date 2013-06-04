using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
	class PlayerMoverAbility : IMoverAbility
	{
		private Regulus.Project.TurnBasedRPG.Serializable.DBEntityInfomation _DBActorInfomation;

		public PlayerMoverAbility(Regulus.Project.TurnBasedRPG.Serializable.DBEntityInfomation dB_actor_infomation)
		{			
			_DBActorInfomation = dB_actor_infomation;
		}
		Types.Vector2 _Start;
		Types.Vector2 _End;
		Types.Vector2 _Vector = new Types.Vector2();
		long _BeginTime;
		double _TotalTime;
		double _Distance ; 
		public void Move(Types.Vector2 end, long time)
		{
			_Start = Regulus.Utility.ValueHelper.DeepCopy(_DBActorInfomation.Property.Position);
			_End = end;
			_BeginTime = time;


			float x = System.Math.Abs(_End.X - _Start.X);
			float y = System.Math.Abs(_End.Y - _Start.Y);
			_Distance = Math.Sqrt(x * x + y * y);

			_Vector.X = x ;
			_Vector.Y = y ;
			_TotalTime = _Distance / _DBActorInfomation.Property.Speed;
		}

		void IMoverAbility.Update(long time, CollisionInformation collision_information)
		{
			
			System.DateTime begin = new DateTime(_BeginTime);
			System.DateTime current = new DateTime(time);
			var seconds = (current - begin).TotalSeconds;
			float newDistPer = (float)(seconds / _TotalTime);
			if (newDistPer < 1.0)
			{
				_DBActorInfomation.Property.Position.X = _Start.X + _Vector.X * newDistPer;
				_DBActorInfomation.Property.Position.X = _Start.Y + _Vector.Y * newDistPer;
			}
		}
	}
}
