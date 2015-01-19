using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///////////////////////////////////////////////////////////////////////////////////////
/// 將物件轉成Serializer的元件
///////////////////////////////////////////////////////////////////////////////////////
namespace Regulus.Serializer
{	
	public class TypeHelper
	{
		public static void Yield()
		{

		}
        public static byte[] StringToByteArray(string str)
        {
            return System.Text.Encoding.Default.GetBytes(str);
        }
		public static byte[] GuidToByteArray(Guid guid)
		{
			return guid.ToByteArray();
		}
		public static byte[] Serializer<T>(T o)
		{            
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<T>(stream, o);
                return stream.ToArray();
            }

		}

        
        public static object DeserializeObject(Type type , byte[] b  )
        {            
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(b))
            {
                return ProtoBuf.Serializer.NonGeneric.Deserialize(type, stream);
            }
        }
		public static T Deserialize<T>(byte[] b)
		{            
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(b))
            {
                return ProtoBuf.Serializer.Deserialize<T>(stream);
            }
		}
		
	}
}
