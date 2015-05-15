using System;
using System.Collections.Generic;

using System.Text;

namespace Regulus
{
	namespace NoSQL
	{
		using MongoDB;
        using MongoDB.Driver;

        using System.Threading.Tasks;

		public class Database
		{



            MongoDB.Driver.MongoClient _Clinet;
            MongoDB.Driver.IMongoDatabase _Database;

            public Database(string mongodb_url)
            {
                //var mongo = new Mongo("mongodb://192.168.40.191:27017");
                _Clinet = new MongoDB.Driver.MongoClient(mongodb_url);
            }

			public void Launch(string name)
			{
                
                _Database = _Clinet.GetDatabase(name);
			}

			IMongoCollection<T> _GetCollection<T>(IMongoDatabase db)
			{
				if (db != null)
				{
                    return db.GetCollection<T>(typeof(T).Name);

				}
				return null;
			}


            public System.Threading.Tasks.Task<List<T>> Find<T>(System.Linq.Expressions.Expression<Func<T, bool>> exp) 
            {
                var coll = _GetCollection<T>(_Database);
                if (coll != null)
                {                    
                    return coll.Find(exp).ToListAsync();
                }
                throw new SystemException("get collection fail.");
            }
            
			public Task Add<T>(T obj) where T : class
			{
				var coll = _GetCollection<T>(_Database);
				if (coll != null)
				{
                    return coll.InsertOneAsync(obj);
				}

                throw new SystemException("get collection fail.");
			}

			public bool Remove<T>(System.Linq.Expressions.Expression<Func<T, bool>> selector) 
			{
				var coll = _GetCollection<T>(_Database);
				if (coll != null)
				{
                    var t = coll.DeleteOneAsync<T>(selector);
                    t.Wait();
                    return t.Result.DeletedCount > 0;
				}

                throw new SystemException("get collection fail.");
			}

			public bool Update<T>(T obj, System.Linq.Expressions.Expression<Func<T, bool>> selector) where T : class
			{

				var coll = _GetCollection<T>(_Database);
				if (coll != null)
				{
					var t = coll.ReplaceOneAsync(selector , obj);
                    t.Wait();
                    return t.Result.MatchedCount > 0;
				}

                throw new SystemException("get collection fail.");
			}

			public void Shutdown()
			{
				
			}



		}
	}
}
