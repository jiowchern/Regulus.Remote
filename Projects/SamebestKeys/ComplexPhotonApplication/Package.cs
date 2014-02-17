using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    namespace Data
    {
        [ProtoBuf.ProtoContract]
        public class Map
        {
            [ProtoBuf.ProtoMember(1)]
            public string Name { get; set; }
            [ProtoBuf.ProtoMember(2)]
            public Entity[] Entitys { get; set; }
        }
        [ProtoBuf.ProtoContract]
        public abstract class Entity
        {
            [ProtoBuf.ProtoMember(1)]
            public Guid Id { get; set; }
        }

        [ProtoBuf.ProtoContract]
        public class StaticEntity : Entity
        {
            [ProtoBuf.ProtoMember(1)]
            public Regulus.Utility.OBB Obb { get; set; }
        }

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



    }


}
namespace Regulus.Project.SamebestKeys
{
    [ProtoBuf.ProtoContract]
    public enum ActionStatue 
    { 
        Idle,
        Greeting,
        Bow,
        Talk,
        Run,
        Happy,
        Sad,
        GangnamStyle
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
    public class AccountInfomation 
    {
        [ProtoBuf.ProtoMember(1)]
        public string Name { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public string Password{ get; set; }
        [ProtoBuf.ProtoMember(3)]
        public Guid Id { get; set; }        
    }

    [ProtoBuf.ProtoContract]
    public class EntityLookInfomation
    {
        [ProtoBuf.ProtoMember(1)]
        public string Name { get; set; }
    }

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
        public Regulus.Types.Vector2 Position { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public int Vision { get; set; }
        [ProtoBuf.ProtoMember(5)]
		public float Speed { get; set; }
        [ProtoBuf.ProtoMember(6)]
        public float Direction { get; set; }
    }

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
    [ProtoBuf.ProtoContract]
    public class MoveInfomation
    {
        // 起始時間 , 啟始位置 , 動作 , 方向 , 位移速度
        [ProtoBuf.ProtoMember(1)]
        public long BeginTime {get;set;}
        [ProtoBuf.ProtoMember(2)]
        public Regulus.Types.Vector2 BeginPosition { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public ActionStatue ActionStatue { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public float MoveDirectionAngle { get; set; }
        [ProtoBuf.ProtoMember(5)]
        public Regulus.Types.Vector2 MoveDirection { get; set; }
        [ProtoBuf.ProtoMember(6)]
        public float Speed { get; set; }
    }

}


