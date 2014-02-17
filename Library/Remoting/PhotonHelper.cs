using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///////////////////////////////////////////////////////////////////////////////////////
/// 將物件轉成Serializer的元件
///////////////////////////////////////////////////////////////////////////////////////
namespace Regulus.PhotonExtension
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

			/*using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
			{
				System.Runtime.Serialization.IFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

				
				formater.Serialize(stream, o);
				
				
				byte[] ret = stream.GetBuffer();				
				return ret;
			}*/
		}
        
		public static T Deserialize<T>(byte[] b)
		{
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(b))
            {
                return ProtoBuf.Serializer.Deserialize<T>(stream);
            }
            /*object obj = null;
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream(b))
			{
				System.Runtime.Serialization.IFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                try
                {
                    obj = formater.Deserialize(stream);
                }
                catch
                { 
                }
				
			}
            return obj;					*/			
		}
		
	}
}
