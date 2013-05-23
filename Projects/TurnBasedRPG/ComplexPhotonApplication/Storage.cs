using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Storage : Samebest.Utility.Singleton<Storage>, Samebest.Game.IFramework  
    {
        Samebest.NoSQL.Database _Database;

        internal void Add(Serializable.AccountInfomation ai)
        {
            _Database.Add(ai);
        }

        void Samebest.Game.IFramework.Launch()
        {
            _Database = new Samebest.NoSQL.Database();
            _Database.Launch("mongodb://127.0.0.1:27017");
        }

        bool Samebest.Game.IFramework.Update()
        {
            return true;
        }

        void Samebest.Game.IFramework.Shutdown()
        {
            if (_Database != null)
                _Database.Shutdown();
        }

        internal Serializable.AccountInfomation FindAccountInfomation(string name)
        {
            var ais = _Database.Query<Serializable.AccountInfomation>();
            var reault = (from a in ais where a.Name == name select a).FirstOrDefault();
            return reault;
        }

        internal bool CheckActorName(string name)
        {
            var ais = _Database.Query<Serializable.DBEntityInfomation>();
            return (from a in ais where a.Look.Name == name select true).FirstOrDefault();
        }

        internal void Add(Serializable.DBEntityInfomation ai)
        {
            _Database.Add(ai);
        }

        internal Serializable.DBEntityInfomation[] FindActor(Guid id)
        {
            var ais = _Database.Query<Serializable.DBEntityInfomation>();
            var result = from a in ais where a.Owner == id select a;
            return result.ToArray();
        }

        internal bool RemoveActor(Guid id, string name)
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

        internal void SaveActor(Serializable.DBEntityInfomation actor)
        {
            _Database.Update<Serializable.DBEntityInfomation>(actor, a => a.Look.Name == actor.Look.Name);
        }
    }
}
