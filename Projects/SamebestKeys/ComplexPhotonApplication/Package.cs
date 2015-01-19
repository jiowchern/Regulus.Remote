using System;

namespace Regulus.Project.SamebestKeys
{
	namespace Data
	{
		[Serializable]
		public struct Stage
		{
			public string MapName { get; set; }            
		}

		[Serializable]
		public struct Scene
		{
            public string Front { get; set; }
			public string Name { get; set; }
			public bool Singleton { get; set; }

			public Stage[] Stages { get; set; }

			public int NumberForPlayer { get; set; }

			public bool NotLimit() {  return NumberForPlayer == 0 ;} 
			
		}
		[Serializable]
		public class Skill
		{
			public int Id { get; set; }
			public int Energy { get; set; }
			public float Begin { get; set; }
			public float Effective { get; set; }
			public float End { get; set; }
			public bool Capture { get; set; }
			public Regulus.CustomType.Rect CaptureBounds { get; set; }
			public int Param1 { get; set; }
			public ActorMode UseMode { get; set; }
		}

		[Serializable]
		[ProtoBuf.ProtoContract]
		public class Map
		{
			
			[ProtoBuf.ProtoMember(1)]
			public string Name { get; set; }
			[ProtoBuf.ProtoMember(2)]
			[System.Xml.Serialization.XmlArrayItem(typeof(Entity))]            
			[System.Xml.Serialization.XmlArrayItem(typeof(PortalEntity))]
			[System.Xml.Serialization.XmlArrayItem(typeof(TriangleEntity))]            
			// 新增的entity需要在這裡定意 ... 
			public Entity[] Entitys { get; set; }


			[ProtoBuf.ProtoMember(3)]
			public Regulus.CustomType.Rect Born;
		}

		[Serializable]
		[ProtoBuf.ProtoContract]
		public abstract class Entity
		{
			[ProtoBuf.ProtoMember(1)]
			public Guid Id { get; set; }
		}

		[Serializable]
		[ProtoBuf.ProtoContract]
		public class PortalEntity : Entity
		{
			public PortalEntity()
			{
				Vision = new CustomType.Rect();
				TargetPosition = new CustomType.Vector2();
			}
			[ProtoBuf.ProtoMember(1)]
			public CustomType.Rect Vision { get; set; }
			[ProtoBuf.ProtoMember(2)]
			public string TargetMap { get; set; }
			[ProtoBuf.ProtoMember(3)]
			public CustomType.Vector2 TargetPosition { get; set; }
		}

		[Serializable]
		public class TriangleEntity : Entity
		{
			public TriangleEntity()
			{
				Polygon = new CustomType.Polygon();
			}
			public CustomType.Polygon Polygon { get; set; }
		}
		
	}


}
namespace Regulus.Project.SamebestKeys
{
    [ProtoBuf.ProtoContract]
    public enum SessionScoreType
    {
        _1,
        _2,
        _3,
        _4,
        _5,
        _6,
    };

    [ProtoBuf.ProtoContract]
    public struct SessionScore
    {
        [ProtoBuf.ProtoMember(1)]
        public SessionScoreType Type;
        [ProtoBuf.ProtoMember(2)]
        public int Score;
    }

	[ProtoBuf.ProtoContract]
	[Flags]
	public enum ActorMode
	{
		None,
		Explore,
		Alert,
		All = Explore | Alert
	}
		
	/// <summary>
	/// 動作狀態
	/// </summary>
	[ProtoBuf.ProtoContract]
	public enum ActionStatue
	{
        Brakes,
		Idle_1,        
		Idle_Long,
		Depression_1,
		Greet_1,
		Happy_1,        
		Run,
		Walk,
        Injury,
        Knockout,
		SkillBegin,
		Skill1,
		Skill2,
		Skill3,
		SkillEnd ,


	}

	[ProtoBuf.ProtoContract]
	public enum LoginResult
	{
		RepeatLogin,
		Error,
		Success
	}

	[ProtoBuf.ProtoContract]
	public class CollisionInformation
	{
	}
}
namespace Regulus.Project.SamebestKeys.Serializable
{

	public class Skill
	{
		public int Id ; 
	}

	[ProtoBuf.ProtoContract]
	public class CrossStatus
	{
		[ProtoBuf.ProtoMember(1)]
		public string SourceMap;
		[ProtoBuf.ProtoMember(2)]
		public Regulus.CustomType.Vector2 SourcePosition;

		[ProtoBuf.ProtoMember(3)]
		public string TargetMap;
		[ProtoBuf.ProtoMember(4)]
		public Regulus.CustomType.Vector2 TargetPosition;
	}

    /*public struct Consumption
    {        
        public int Number;
        public int Coins;
        public int Source;
        public string Remark;
    }*/
	public class AccountInfomation 
	{        		
		public string Name { get; set; }
		
		public string Password{ get; set; }		
		public Guid Id { get; set; }

		public string[] LevelRecords { get; set; }        
             
	}

	/// <summary>
	/// Entity外表資訊
	/// </summary>
	[ProtoBuf.ProtoContract]
	public class EntityLookInfomation
	{
		[ProtoBuf.ProtoMember(1)]
		public string Name { get; set; }
		[ProtoBuf.ProtoMember(2)]
		public int Shell { get; set; }
	}



	/// <summary>
	/// Entity屬性
	/// </summary>
	
	public class EntityPropertyInfomation
	{
        [Flags]
        public enum IDENTITY
        {
            GUEST = 0,            
            STUNDENT = 1,
            CONVERSATION = 2,            
        };
        
		public EntityPropertyInfomation()
		{            
			MaxEnergy = 50;
			Energy = 50;
			Skills = new System.Collections.Generic.List<Skill>();
			Position = new CustomType.Vector2();
            Identity = IDENTITY.GUEST;
		}
		
		public Guid Id { get; set; }
		
		public string Map { get; set; }
		
		
		public int Vision { get; set; }
		
		public float Speed { get; set; }
		
		public float Direction { get; set; }

		
		public Regulus.CustomType.Vector2 Position { get; set; }


		
		public float MaxEnergy { get; set; }


        public float Energy { get; set; }

		
		public bool Died { get; set; }
		
		public System.Collections.Generic.List<Skill> Skills { get; set; }

        public IDENTITY Identity { get; set; }
		
	}

	/// <summary>
	/// Entity資訊
	/// </summary>
	[ProtoBuf.ProtoContract]
	public class EntityInfomation
	{
		public EntityInfomation()
		{
			Position = new CustomType.Vector2();            
		}
		[ProtoBuf.ProtoMember(1)]
		public Guid Id { get; set; }
		[ProtoBuf.ProtoMember(2)]
		public Regulus.CustomType.Vector2 Position { get; set; }        
	}


    
    public class Record
    {
        public Record()
		{
			Clearances = new string[1] { "" };
		}
        public string[] Clearances;
    }

	
	public class DBEntityInfomation 
	{
		public DBEntityInfomation()
		{
			Look = new EntityLookInfomation();
			Property = new EntityPropertyInfomation();
		}
		
		public EntityPropertyInfomation Property { get; set; }
		
		public EntityLookInfomation Look { get; set; }
		
		public Guid Owner { get; set; }    
    
        public Record Record { get; set; }  
	}

	[ProtoBuf.ProtoContract]
	public class ActionCommand
	{        
		[ProtoBuf.ProtoMember(1)]
		public ActionStatue Command { get; set; }
		[ProtoBuf.ProtoMember(2)]
		public float Speed { get; set; }
		[ProtoBuf.ProtoMember(3)]
		public float Direction { get; set; }
		[ProtoBuf.ProtoMember(4)]
		public bool Turn { get; set; }
		[ProtoBuf.ProtoMember(5)]
		public bool Absolutely { get; set; }

        [ProtoBuf.ProtoMember(6)]
        public long Time { get; set; }
	}
	/// <summary>
	/// 移動資訊
	/// </summary>
	[ProtoBuf.ProtoContract]
	public class MoveInfomation
	{
		/// <summary>
		/// 起始時間
		/// </summary>
		[ProtoBuf.ProtoMember(1)]
		public long BeginTime {get;set;}

		/// <summary>
		/// 啟始位置
		/// </summary>
		[ProtoBuf.ProtoMember(2)]
		public Regulus.CustomType.Vector2 BeginPosition { get; set; }

		/// <summary>
		/// 動作狀態
		/// </summary>
		[ProtoBuf.ProtoMember(3)]
		public ActionStatue ActionStatue { get; set; }

		/// <summary>
		/// 移動角度
		/// </summary>
		[ProtoBuf.ProtoMember(4)]
		public float MoveDirectionAngle { get; set; }

		/// <summary>
		/// 移動方向
		/// </summary>
		[ProtoBuf.ProtoMember(5)]
		public Regulus.CustomType.Vector2 MoveDirection { get; set; }

		/// <summary>
		/// 位移速度
		/// </summary>
		[ProtoBuf.ProtoMember(6)]
		public float Speed { get; set; }

		[ProtoBuf.ProtoMember(7)]
		public ActorMode Mode { get; set; }
	}

}


