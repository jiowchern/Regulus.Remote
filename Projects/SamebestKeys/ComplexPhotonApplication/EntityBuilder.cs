using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
	interface IEntityBuilder<TData , TEntity>
	{
		TEntity Build(TData source);		
	}

	class EntityBuilder : Regulus.Utility.Singleton<EntityBuilder>	
    , IEntityBuilder<Data.PortalEntity, PortalEntity>
    , IEntityBuilder<Data.TriangleEntity, TriangleEntity>
	{
		delegate Entity CommonBuilder(Data.Entity data);
		Dictionary<Type, CommonBuilder> _Builders;

		public EntityBuilder()
		{
			_Builders = new Dictionary<Type, CommonBuilder >();			
            _Builders.Add(typeof(Data.PortalEntity), _Builder((this as IEntityBuilder<Data.PortalEntity, PortalEntity>)));
            _Builders.Add(typeof(Data.TriangleEntity), _Builder((this as IEntityBuilder<Data.TriangleEntity, TriangleEntity>)));
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

        PortalEntity IEntityBuilder<Data.PortalEntity, PortalEntity>.Build(Data.PortalEntity source)
        {
            var rect = new Regulus.Types.Rect(source.Vision.Left ,
                source.Vision.Top,
                source.Vision.Right - source.Vision.Left,
                source.Vision.Bottom - source.Vision.Top
                );                        
            return new PortalEntity(source.Id, rect, source.TargetMap, source.TargetPosition);
        }

        TriangleEntity IEntityBuilder<Data.TriangleEntity, TriangleEntity>.Build(Data.TriangleEntity source)
        {
            return new TriangleEntity(source.Id, source.Polygon);
        }
    }
}
