using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class Storage :  Regulus.Utility.IUpdatable, IStorage
    {
        Regulus.NoSQL.Database _Database;

        public void Add(Serializable.AccountInfomation ai)
        {
            _Database.Add(ai);
        }


		void Regulus.Framework.ILaunched.Launch()
        {
            _Database = new Regulus.NoSQL.Database();
            _Database.Launch("mongodb://127.0.0.1:27017", "SamebestKeys");
            
        }

		bool Regulus.Utility.IUpdatable.Update()
        {
            return true;
        }

		void Regulus.Framework.ILaunched.Shutdown()
        {
            if (_Database != null)
                _Database.Shutdown();
        }

        public Serializable.AccountInfomation FindAccountInfomation(string name)
        {
            var ais = _Database.Query<Serializable.AccountInfomation>();
            var reault = (from a in ais where a.Name == name select a).FirstOrDefault();
            return reault;
        }

        public bool CheckActorName(string name)
        {
            var ais = _Database.Query<Serializable.DBEntityInfomation>();
            return (from a in ais where a.Look.Name == name select true).FirstOrDefault();
        }

        public void Add(Serializable.DBEntityInfomation ai)
        {
            _Database.Add(ai);
        }

        public Serializable.DBEntityInfomation[] FindActor(Guid id)
        {
            var ais = _Database.Query<Serializable.DBEntityInfomation>();
            var result = from a in ais where a.Owner == id select a;
            return result.ToArray();
        }

        public bool RemoveActor(Guid id, string name)
        {
            var ais = _Database.Query<Serializable.DBEntityInfomation>();
            var result = (from a in ais where a.Owner == id && a.Look.Name == name select a).FirstOrDefault();
            if (result != null)
            {
                _Database.Remove(result);
                return true;
            }
            return false;
        }

        public void SaveActor(Serializable.DBEntityInfomation actor)
        {
            _Database.Update<Serializable.DBEntityInfomation>(actor, a => a.Look.Name == actor.Look.Name);
        }
    }
}
