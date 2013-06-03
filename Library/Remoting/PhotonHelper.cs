using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///////////////////////////////////////////////////////////////////////////////////////
/// 將物件轉成Serializer的元件
///////////////////////////////////////////////////////////////////////////////////////
namespace Samebest.PhotonExtension
{	
	public class TypeHelper
	{
		public static void Yield()
		{

		}
		public static byte[] GuidToByteArray(Guid guid)
		{
			return guid.ToByteArray();
		}
		public static byte[] Serializer(object o)
		{
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
			{
				System.Runtime.Serialization.IFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

				
				formater.Serialize(stream, o);
				
				
				byte[] ret = stream.GetBuffer();				
				return ret;
			}
		}
		public static byte[] SerializerZip(object obj)
		{
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
			{
				System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();//定义BinaryFormatter以序列化object对象  


                byte[] buffer = null;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    formatter.Serialize(ms, obj);//把object对象序列化到内存流  
                    buffer = ms.ToArray();//把内存流对象写入字节数组                      
                }

                using (System.IO.MemoryStream msNew = new System.IO.MemoryStream())
                {
                    System.IO.Compression.GZipStream gzipStream = new System.IO.Compression.GZipStream(msNew, System.IO.Compression.CompressionMode.Compress, true);//创建压缩对象  

                    gzipStream.Write(buffer, 0, buffer.Length);//把压缩后的数据写入文件  

                    gzipStream.Close();//关闭压缩流,这里要注意：一定要关闭，要不然解压缩的时候会出现小于4K的文件读取不到数据，大于4K的文件读取不完整              

                    gzipStream.Dispose();//释放对象  

                    return msNew.ToArray();
                }
			}
		}
		public static object Deserialize(byte[] b)
		{
            object obj = null;
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream(b))
			{
				System.Runtime.Serialization.IFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				
				obj = formater.Deserialize(stream);
			}
            return obj;								
		}
		public static object DeserializeZip(byte[] bytes) 
		{
			System.IO.MemoryStream msNew = new System.IO.MemoryStream(bytes);

			msNew.Position = 0;

			System.IO.Compression.GZipStream gzipStream = new System.IO.Compression.GZipStream(msNew, System.IO.Compression.CompressionMode.Decompress);//创建解压对象  

			byte[] buffer = new byte[4096];//定义数据缓冲  

			int offset = 0;//定义读取位置  

			System.IO.MemoryStream ms = new System.IO.MemoryStream();//定义内存流  

			while ((offset = gzipStream.Read(buffer, 0, buffer.Length)) != 0)
			{

				ms.Write(buffer, 0, offset);//解压后的数据写入内存流  

			}

			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter sfFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();//定义BinaryFormatter以反序列化object对象  

			ms.Position = 0;//设置内存流的位置  

			object obj;

			try
			{

				obj = (object)sfFormatter.Deserialize(ms);//反序列化  

			}

			catch
			{

				throw;

			}

			finally
			{

				ms.Close();//关闭内存流  

				

			}

			gzipStream.Close();//关闭解压缩流  

			

			msNew.Close();

			msNew.Dispose();

			return obj;
		}
	}
}
