using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility.IO
{
	public static class Serialization
	{
		static public void Write<T>(T obj , string path)
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

			System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create);

			formatter.Serialize(fs, obj);

			fs.Close();				
		}

		static public T Read<T>(string path)
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formmater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

			System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open);

			T obj = (T)formmater.Deserialize(fs);

			fs.Close();

			return obj;
		}
		static public byte[] Serializer(object o)
		{
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
			{
				System.Runtime.Serialization.IFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();


				formater.Serialize(stream, o);

				byte[] ret = stream.GetBuffer();
				
				return ret;
			}
		}
		static public object Deserialize(byte[] b)
		{
			object obj = null;
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream(b))
			{
				System.Runtime.Serialization.IFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

				obj = formater.Deserialize(stream);
			}
			return obj;
		}
	}
}
