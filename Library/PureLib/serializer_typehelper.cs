using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Regulus
{
	public class TypeHelper
	{
		public static void Yield()
		{
		}

		public static byte[] StringToByteArray(string str)
		{
			return Encoding.Default.GetBytes(str);
		}

		public static byte[] GuidToByteArray(Guid guid)
		{
			return guid.ToByteArray();
		}

		public static byte[] Serializer<T>(T o)
		{

            /*BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, o);
                return ms.ToArray();
            }*/

            using (var stream = new MemoryStream())
			{
				ProtoBuf.Serializer.Serialize(stream, o);
				return stream.ToArray();
			}
		}

		public static object DeserializeObject(Type type, byte[] b)
		{
		    try
		    {
                /*BinaryFormatter bf = new BinaryFormatter();
                using (var ms = new MemoryStream(b))
                {                    
                    var obj = bf.Deserialize(ms);
                    return obj;
                }*/
                using (var stream = new MemoryStream(b))
                {
                    return ProtoBuf.Serializer.NonGeneric.Deserialize(type, stream);
                }
            }
		    catch(Exception e )
		    {		        
		        throw new DeserializeException(e);
		    }
			
		}

		public static T Deserialize<T>(byte[] b)
		{
            /*BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream(b))
            {
                var obj = bf.Deserialize(ms);
                return (T)obj;
            }*/

            using (var stream = new MemoryStream(b))
			{
				return ProtoBuf.Serializer.Deserialize<T>(stream);
			}
		}
	}
}
