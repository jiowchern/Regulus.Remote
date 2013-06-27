using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComplexApplication
{
	class Storage
	{
		Samebest.NoSQL.Database _Database;
		internal void Initial()
		{
			_Database = new Samebest.NoSQL.Database();
            _Database.Launch("mongodb://127.0.0.1:27017" , "Crystal");
		}

		internal void Finial()
		{
			_Database.Shutdown();
		}
	}
}
