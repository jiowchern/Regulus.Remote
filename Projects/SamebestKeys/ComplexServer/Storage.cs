using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class Storage :  Regulus.Utility.IUpdatable, IStorage
    {
        Regulus.NoSQL.Database _Game;
        Regulus.SQL.Database _Business;
        

        public void Add(Serializable.AccountInfomation ai)
        {
            _Game.Add(ai);
        }


		void Regulus.Framework.ILaunched.Launch()
        {
            _Game = new Regulus.NoSQL.Database();
            _Game.Launch("mongodb://127.0.0.1:27017", "SamebestKeys");

            _Business = new SQL.Database();
            //_Business.Connect("localhost", "root", "keys", "SamebestKeys");
        }

		bool Regulus.Utility.IUpdatable.Update()
        {
            return true;
        }

		void Regulus.Framework.ILaunched.Shutdown()
        {
            if (_Game != null)
                _Game.Shutdown();

            _Business.Disconnect();
        }


        public void AddConsumptionCoins(Guid owner , int coins)
        {
            var number = _FindPlayerNumber(owner);
            _AddConsumptionCoins(number, coins);    
        }

        private void _AddConsumptionCoins(int number, int coins)
        {
            var sql = "insert into Consumption(Number, Coins , Source , Remark) values (" + number.ToString() + "," + coins.ToString() + ",'GameSystem','')";
            _Business.ExecuteNonQuery(sql);
        }
        public void CreateConsumptionPlayer(Guid owner)
        {
            return;
            if(_HasConsumptionPlayer(owner) == false)
            {
                _CreateConsumptionPlayer( owner);
            }
        }

        

        private bool _HasConsumptionPlayer(Guid owner)
        {
            var sql = "select * from Player where Id = '" + owner.ToString() + "'";
            using(var data = _Business.Execute(sql))
            {
                return data.HasRows;
            }
        }

        private void _CreateConsumptionPlayer(Guid owner)
        {
            var sql = "insert into Player (Id) values ('"+ owner.ToString() +"')";
            _Business.ExecuteNonQuery(sql);
        }
        public int QueryConsumptionCoins(Guid owner)
        {

            var number = _FindPlayerNumber(owner);

            var sql = "select SUM(Coins) from Consumption where Number = " + number.ToString();
            using(var data = _Business.Execute(sql))
            {
                if (data.HasRows )
                {
                    data.Read();
                    if(data.IsDBNull(0) == false)
                    {
                        var coinsAmount = data.GetInt32(0);
                        return coinsAmount;
                    }
                        
                }
            }
            
            return 0;
            
        }

        private int _FindPlayerNumber(Guid owner)
        {
            var sql = "select Number from Player where Id = '" + owner.ToString()+"'";
            using(var data = _Business.Execute(sql))
            {
                data.Read();
                return (int)data[0];
            }
        }
        public Serializable.AccountInfomation FindAccountInfomation(string name)
        {
            var ais = _Game.Query<Serializable.AccountInfomation>();
            var reault = (from a in ais where a.Name == name select a).FirstOrDefault();
            return reault;
        }

        public bool CheckActorName(string name)
        {
            var ais = _Game.Query<Serializable.DBEntityInfomation>();
            return (from a in ais where a.Look.Name == name select true).FirstOrDefault();
        }

        public void Add(Serializable.DBEntityInfomation ai)
        {
            _Game.Add(ai);
        }

        public Serializable.DBEntityInfomation[] FindActor(Guid id)
        {
            var ais = _Game.Query<Serializable.DBEntityInfomation>();
            var result = from a in ais where a.Owner == id select a;
            return result.ToArray();
        }

        public bool RemoveActor(Guid id, string name)
        {
            var ais = _Game.Query<Serializable.DBEntityInfomation>();
            var result = (from a in ais where a.Owner == id && a.Look.Name == name select a).FirstOrDefault();
            if (result != null)
            {
                _Game.Remove(result);
                return true;
            }
            return false;
        }

        public void SaveActor(Serializable.DBEntityInfomation actor)
        {
            _Game.Update<Serializable.DBEntityInfomation>(actor, a => a.Look.Name == actor.Look.Name);
        }
    }
}
