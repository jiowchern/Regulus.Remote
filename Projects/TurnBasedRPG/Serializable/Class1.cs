using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.TurnBasedRPG
{
    [Serializable]
    public enum LoginResult
    {
        RepeatLogin,
        Error,
        Success
    }
}
namespace Regulus.Project.TurnBasedRPG.Serializable
{
    [Serializable]
    public class AccountInfomation 
    {
        public string Name { get; set; }
        public string Password{ get; set; }
        public Guid Id { get; set; }        
    }

    [Serializable]
    public class EntityLookInfomation
    {
        public string Name { get; set; }
    }

    [Serializable]
    public class EntityPropertyInfomation
    {
        public EntityPropertyInfomation()
        { 
            Position = new Types.Vector2();
        }

        public Guid Id { get; set; }
        public Regulus.Types.Vector2 Position { get; set; }
        public int Vision { get; set; }
    }

    [Serializable]
    public class EntityInfomation
    {
        public EntityInfomation()
        {
            Position = new Types.Vector2();            
        }
        public Guid Id { get; set; }
        public Regulus.Types.Vector2 Position { get; set; }        
    }


    [Serializable]
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
    }


}
