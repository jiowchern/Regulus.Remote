using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessServer
{
    class Storage
    {
        Regulus.NoSQL.Database _Database;
        internal Account FindAccount(string account, string password)
        {
            return null;
            //var sql = "select * from Player where Id = '" + owner.ToString() + "'";
        }


        internal void PushCoins(int _Id, string player_id, int coins, string remark)
        {
            throw new NotImplementedException();
        }
    }
}
