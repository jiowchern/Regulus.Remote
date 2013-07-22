using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
	interface IEntityBuilder<TData , TEntity>
	{
		TEntity Build(TData source);		
	}

	class EntityBuilder : Regulus.Utility.Singleton<EntityBuilder>
	, IEntityBuilder<Data.StaticEntity , StaticEntity>
	{
		delegate Entity CommonBuilder(Data.Entity data);
		Dictionary<Type, CommonBuilder> _Builders;

		public EntityBuilder()
		{
			_Builders = new Dictionary<Type, CommonBuilder >();
			_Builders.Add(typeof(Data.StaticEntity), _Builder((this as IEntityBuilder<Data.StaticEntity, StaticEntity>)) );
		}

		private CommonBuilder _Builder<TData, TEntity>(IEntityBuilder<TData, TEntity> entity_builder) 
			where TData  : Data.Entity
			where TEntity : Entity
		{
			CommonBuilder b = (data)=>
			{
				var d = data as TData;				
				return entity_builder.Build(d) as Entity;				
			};
			return b;
		}


		internal Entity Build(Data.Entity ent)
		{
			CommonBuilder builder = _Find(ent.GetType());
			if(builder != null)
			{
				return builder(ent);
			}
			throw new SystemException("沒有對應的Entity Builder! [" + ent.GetType() + "]");
		}

		private CommonBuilder _Find(Type type)
		{
			CommonBuilder builder;
			if(_Builders.TryGetValue(type, out builder))
			{
				return builder ;
			}
			return null;
		}



		StaticEntity IEntityBuilder<Data.StaticEntity, StaticEntity>.Build(Data.StaticEntity source)
		{
			return new StaticEntity(source.Obb, source.Id);
		}
	}
	
	
	
}
