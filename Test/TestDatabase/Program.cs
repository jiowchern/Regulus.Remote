using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            Samebest.NoSQL.Database db = new Samebest.NoSQL.Database();
            db.Launch("mongodb://127.0.0.1:27017");
            db.Shutdown();
        }
    }
}
