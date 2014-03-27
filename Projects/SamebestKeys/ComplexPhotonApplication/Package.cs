using System;

namespace Regulus.Project.SamebestKeys
{
    namespace Data
    {
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
                Vision = new Types.Rect();
                TargetPosition = new Types.Vector2();
            }
            [ProtoBuf.ProtoMember(1)]
            public Types.Rect Vision { get; set; }
            [ProtoBuf.ProtoMember(2)]
            public string TargetMap { get; set; }
            [ProtoBuf.ProtoMember(3)]
            public Types.Vector2 TargetPosition { get; set; }
        }

        [Serializable]
        public class TriangleEntity : Entity
        {
            public TriangleEntity()
            {
                Polygon = new Types.Polygon();
            }
            public Types.Polygon Polygon { get; set; }
        }
        
    }


}
namespace Regulus.Project.SamebestKeys
{
	/// <summary>
	/// 動作狀態
	/// </summary>
    [ProtoBuf.ProtoContract]
    public enum ActionStatue 
    { 
        Idle,
        Angry,
        Call,
        Greet,
        Walk,
        Happy,
        individual,
        
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

    [ProtoBuf.ProtoContract]
    public class CrossStatus
    {
        [ProtoBuf.ProtoMember(1)]
        public string SourceMap;
        [ProtoBuf.ProtoMember(2)]
        public Regulus.Types.Vector2 SourcePosition;

        [ProtoBuf.ProtoMember(3)]
        public string TargetMap;
        [ProtoBuf.ProtoMember(4)]
        public Regulus.Types.Vector2 TargetPosition;
    }

    [ProtoBuf.ProtoContract]
    public class AccountInfomation 
    {
        [ProtoBuf.ProtoMember(1)]
        public string Name { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public string Password{ get; set; }
        [ProtoBuf.ProtoMember(3)]
        public Guid Id { get; set; }        
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
    [ProtoBuf.ProtoContract]
    public class EntityPropertyInfomation
    {
        public EntityPropertyInfomation()
        { 
            Position = new Types.Vector2();
            Map = "";
        }
        [ProtoBuf.ProtoMember(1)]
        public Guid Id { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public string Map { get; set; }
        
        [ProtoBuf.ProtoMember(3)]
        public int Vision { get; set; }
        [ProtoBuf.ProtoMember(4)]
		public float Speed { get; set; }
        [ProtoBuf.ProtoMember(5)]
        public float Direction { get; set; }

        [ProtoBuf.ProtoMember(6)]
        public Regulus.Types.Vector2 Position { get; set; }
    }

	/// <summary>
	/// Entity資訊
	/// </summary>
    [ProtoBuf.ProtoContract]
    public class EntityInfomation
    {
        public EntityInfomation()
        {
            Position = new Types.Vector2();            
        }
        [ProtoBuf.ProtoMember(1)]
        public Guid Id { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public Regulus.Types.Vector2 Position { get; set; }        
    }


    [ProtoBuf.ProtoContract]
    public class DBEntityInfomation 
    {
        public DBEntityInfomation()
        {
            Look = new EntityLookInfomation();
            Property = new EntityPropertyInfomation();
        }
        [ProtoBuf.ProtoMember(1)]
        public EntityPropertyInfomation Property { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public EntityLookInfomation Look { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public Guid Owner { get; set; }        
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
        public Regulus.Types.Vector2 BeginPosition { get; set; }

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
        public Regulus.Types.Vector2 MoveDirection { get; set; }

		/// <summary>
		/// 位移速度
		/// </summary>
        [ProtoBuf.ProtoMember(6)]
        public float Speed { get; set; }
    }

}


