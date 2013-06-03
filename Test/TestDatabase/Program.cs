using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDatabase
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
        public string Password { get; set; }
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



    class Program
    {
        static void Main(string[] args)
        {
            DBEntityInfomation db1 = new DBEntityInfomation();
            db1.Property.Position.X = 100;
            DBEntityInfomation db2 = new DBEntityInfomation();
            db2.Property.Position.X = 100;
            db2.Look.Name = "dff";
            var ret = Regulus.Utility.ValueHelper.DeepEqual(db1, db2);
            Samebest.NoSQL.Database db = new Samebest.NoSQL.Database();
            db.Launch("mongodb://127.0.0.1:27017");
            db.Shutdown();
        }
    }
}
