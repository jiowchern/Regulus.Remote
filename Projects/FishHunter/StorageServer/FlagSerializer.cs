/*using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Storage
{
    class FlagSerializer<T> : MongoDB.Bson.Serialization.Serializers.BsonBaseSerializer 
    {
        public FlagSerializer()
        {

        }
        //
        // 摘要: 
        //     Serializes an object to a BsonWriter.
        //
        // 參數: 
        //   bsonWriter:
        //     The BsonWriter.
        //
        //   nominalType:
        //     The nominal type.
        //
        //   value:
        //     The object.
        //
        //   options:
        //     The serialization options.
        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (value == null)
            {
                bsonWriter.WriteNull();
            }
            else
            {
                var flag = (Regulus.CustomType.Flag<T>)value;

                bsonWriter.WriteStartArray();
                foreach (var element in flag)
                {
                    bsonWriter.WriteStartDocument();                                        
                    BsonSerializer.Serialize(bsonWriter, typeof(T), element);
                    bsonWriter.WriteEndDocument();
                }
                bsonWriter.WriteEndArray();
            }
        }

        //
        // 摘要: 
        //     Deserializes an object from a BsonReader.
        //
        // 參數: 
        //   bsonReader:
        //     The BsonReader.
        //
        //   nominalType:
        //     The nominal type of the object.
        //
        //   actualType:
        //     The actual type of the object.
        //
        //   options:
        //     The serialization options.
        //
        // 傳回: 
        //     An object.
        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            var bsonType = bsonReader.CurrentBsonType;
            if (bsonType == BsonType.Null)
            {
                bsonReader.ReadNull();
                return null;
            } 
            else if (bsonType == BsonType.Array) {

                var flag = new Regulus.CustomType.Flag<T>();
				bsonReader.ReadStartArray();
                
                var valueDiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(T));
				while (bsonReader.ReadBsonType() != BsonType.EndOfDocument) 
                {
					bsonReader.ReadStartDocument();
					
					bsonReader.ReadBsonType();
					var valueType = valueDiscriminatorConvention.GetActualType(bsonReader,typeof(T));
					var valueSerializer = BsonSerializer.LookupSerializer(valueType);
					var value = (T) valueSerializer.Deserialize(bsonReader,typeof(T), valueType, null);
					bsonReader.ReadEndDocument();
                    flag[value] = true;
					
				}
				bsonReader.ReadEndArray();
                return flag;
			} else {
				var message = string.Format("Can't deserialize a {0} from BsonType{1}.", nominalType.FullName, bsonType);
				throw new SystemException(message);
			}

        }

        
    }
}
*/