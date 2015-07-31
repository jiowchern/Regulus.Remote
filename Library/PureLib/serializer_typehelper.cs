// --------------------------------------------------------------------------------------------------------------------
// <copyright file="serializer_typehelper.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the TypeHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
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
			using (var stream = new MemoryStream())
			{
				ProtoBuf.Serializer.Serialize(stream, o);
				return stream.ToArray();
			}
		}

		public static object DeserializeObject(Type type, byte[] b)
		{
			using (var stream = new MemoryStream(b))
			{
				return ProtoBuf.Serializer.NonGeneric.Deserialize(type, stream);
			}
		}

		public static T Deserialize<T>(byte[] b)
		{
			using (var stream = new MemoryStream(b))
			{
				return ProtoBuf.Serializer.Deserialize<T>(stream);
			}
		}
	}
}