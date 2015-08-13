using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;


using ProtoBuf;

namespace Regulus.Utility
{
	public static class Serialization
	{
		public static void Write<T>(T obj, string path)
		{
			// System.IO.File.WriteAllBytes(path, _WriteProtobuf<T>(obj));
			File.WriteAllText(path, Serialization._WriteXml(obj));
		}

		private static byte[] _WriteProtobuf<T>(T obj)
		{
			using(var stream = new MemoryStream())
			{
				Serializer.Serialize(stream, obj);
				return stream.ToArray();
			}
		}

		private static string _WriteXml<T>(T obj)
		{
			using(var stream = new StringWriter())
			{
				var x = new XmlSerializer(typeof(T));
				x.Serialize(stream, obj);
				return stream.ToString();
			}
		}

		private static void _WriteBinrary<T>(T obj, string path)
		{
			var formatter = new BinaryFormatter();

			var fs = new FileStream(path, FileMode.Create);

			formatter.Serialize(fs, obj);

			fs.Close();
		}

		public static T Read<T>(byte[] stream)
		{
			// return _ReadBinraryStream<T>(stream);
			return Serialization._ReadXmlStream<T>(stream);
		}

		private static T _ReadXmlStream<T>(byte[] buffer)
		{
			using(var stream = new StringReader(Encoding.Default.GetString(buffer)))
			{
				var ser = new XmlSerializer(typeof(T));
				return (T)ser.Deserialize(stream);
			}
		}

		private static T _ReadBinraryStream<T>(byte[] stream)
		{
			var formmater = new BinaryFormatter();

			var ms = new MemoryStream(stream);

			var obj = (T)formmater.Deserialize(ms);

			ms.Close();
			return obj;
		}

		public static T Read<T>(string path)
		{
			// return _ReadBinrary<T>(path);
			return Serialization._ReadXmlStream<T>(File.ReadAllBytes(path));
		}

		private static T _ReadBinrary<T>(string path)
		{
			var formmater = new BinaryFormatter();

			var fs = new FileStream(path, FileMode.Open);

			var obj = (T)formmater.Deserialize(fs);

			fs.Close();
			return obj;
		}
	}
}
