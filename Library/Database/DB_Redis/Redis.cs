using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using NServiceKit.Redis;

namespace Regulus.Database.DB_Redis
{
	public class Database
	{
		public class Data
		{
			public enum NAME
			{
				MASTER,

				SLAVE_1,

				COUNT
			}

			public NAME Name { get; set; }

			public IRedisClient RedisClient { get; set; }
		}

		private List<Data> _DbGroup;

		//private PooledRedisClientManager _RedisManager;
		public void Launch(string name)
		{
			_DbGroup = new List<Data>
			{
				new Data
				{
					Name = Data.NAME.MASTER,
					RedisClient = new PooledRedisClientManager("127.0.0.1:6379").GetClient()
				},
				new Data
				{
					Name = Data.NAME.SLAVE_1,
					RedisClient = new PooledRedisClientManager("127.0.0.1:6380").GetClient()
				}
			};
		}

		public void Shutdown()
		{
			foreach(var data in _DbGroup)
			{
				data.RedisClient.Shutdown();
			}
		}

		public void FlushDb()
		{
			foreach(var data in _DbGroup)
			{
				data.RedisClient.FlushDb();
			}
		}

		/// <summary>
		/// </summary>
		public void Add<T, TId>(T obj, Expression<Func<T, TId>> exp) where T : class
		{
			var propertyInfo = _GetPropertyInfo(exp);

			var db = _GetDB(Data.NAME.MASTER);

			var redis = db.RedisClient.As<T>();

			var id = redis.GetNextSequence();

			propertyInfo.GetSetMethod().Invoke(obj, new object[]
			{
				id
			});

			redis.Store(obj);
		}

		/// <summary>
		///     
		/// </summary>
		public bool Update<T, TId>(T source, Expression<Func<T, TId>> exp) where T : class
		{
			//var propertyInfo = _GetPropertyInfo(exp);

			//(long)propertyInfo.GetGetMethod().Invoke(source, null);

			var db = _GetDB(Data.NAME.MASTER);

			var redis = db.RedisClient.As<T>();

			redis.Store(source);

			return true;
		}

		/// <summary>
		/// </summary>
		public bool Remove<T, TId>(T obj, Expression<Func<T, TId>> exp) where T : class
		{
			var propertyInfo = _GetPropertyInfo(exp);

			var id = (long)propertyInfo.GetGetMethod().Invoke(obj, null);

			var db = _GetDB(Data.NAME.MASTER);

			var redis = db.RedisClient.As<T>();

			redis.DeleteById(id);

			return true;
		}

		/// <summary>
		/// </summary>
		public T Find<T, TId>(T source, Expression<Func<T, TId>> exp) where T : class
		{
			var propertyInfo = _GetPropertyInfo(exp);

			var id = (long)propertyInfo.GetGetMethod().Invoke(source, null);

			var db = _GetDB(Data.NAME.SLAVE_1);

			var redis = db.RedisClient.As<T>();

			return redis.GetById(id);
		}

		/// <summary>
		/// </summary>
		public List<T> FindAll<T>()
		{
			var db = _GetDB(Data.NAME.SLAVE_1);

			var redis = db.RedisClient.As<T>();

			return (List<T>)redis.GetAll();
		}

		private PropertyInfo _GetPropertyInfo<T, TId>(Expression<Func<T, TId>> exp) where T : class
		{
			if(exp.Body.NodeType != ExpressionType.MemberAccess)
			{
				throw new ArgumentException("不支援的nodetype");
			}

			var propertyInfo = ((MemberExpression)exp.Body).Member as PropertyInfo;

			if(propertyInfo == null)
			{
				throw new ArgumentException("只支援 Property");
			}

			if(propertyInfo.Name != "Id")
			{
				throw new ArgumentException("找不到欄位 = Id");
			}
			return propertyInfo;
		}

		private Data _GetDB(Data.NAME name)
		{
			var db = _DbGroup.SingleOrDefault(x => x.Name == name);
			if(db == null)
			{
				throw new ArgumentException("請檢查db設定");
			}
			return db;
		}
	}
}
