using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMySql
{
    class Program
    {
        static void Main(string[] args)
        {
            Regulus.SQL.Database db = new Regulus.SQL.Database();
            db.Connect("localhost", "root", "keys", "test");
            
            //db.Connect("localhost:3306", "root", "keys", "test");
            db.Disconnect();
        }
    }
}
