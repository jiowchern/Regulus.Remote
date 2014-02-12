using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus
{
	namespace NoSQL
	{
		using MongoDB;

		public class Database
		{
			
			Mongo _Mongo;
			IMongoDatabase _Database;

			

			public void Launch(string mongodb_url , string name)
			{
				//var mongo = new Mongo("mongodb://192.168.40.191:27017");
				var mongo = new Mongo(mongodb_url);
				_Mongo = mongo;


				_Mongo.Connect();
				_Database = _Mongo.GetDatabase(name);
			}

			IMongoCollection<T> _GetCollection<T>(IMongoDatabase db) where T : class
			{
				if (db != null)
				{
					return db.GetCollection<T>();

				}
				return null;
			}
			public IQueryable<T> Linq<T>() where T : class
			{
				var coll = _GetCollection<T>(_Database);
				if (coll != null)
				{
					return coll.Linq();
				}
				return null;
			}

			public T[] Query<T>() where T : class
			{
				var coll = _GetCollection<T>(_Database);
				if (coll != null)
				{
					return coll.FindAll().Documents.ToArray<T>();
				}
				return null;
			}

			public void Add<T>(T obj) where T : class
			{
				var coll = _GetCollection<T>(_Database);
				if (coll != null)
				{
					coll.Insert(obj);
				}
			}

			public void Remove<T>(T obj) where T : class
			{
				var coll = _GetCollection<T>(_Database);
				if (coll != null)
				{
					coll.Remove(obj);
				}
			}

			public void Update<T>(T obj, System.Linq.Expressions.Expression<Func<T, bool>> selector) where T : class
			{

				var coll = _GetCollection<T>(_Database);
				if (coll != null)
				{
					coll.Update<T>(obj, selector, true);
				}
			}

			public void Shutdown()
			{
				if (_Mongo != null)
				{
					_Database = null;
					_Mongo.Disconnect();
					_Mongo = null;
				}
			}



		}
	}
}
