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
            
            //System.IO.File.WriteAllBytes(path, _WriteProtobuf<T>(obj));
            System.IO.File.WriteAllText(path, _WriteXml<T>(obj));
        }

        private static byte[] _WriteProtobuf<T>(T obj)
        {
            using(System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<T>(stream, obj);
                return stream.ToArray();
            }
            
        }

        private static string _WriteXml<T>(T obj)
        {

            using (System.IO.StringWriter stream = new System.IO.StringWriter())            
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(T));
                x.Serialize(stream, obj);
                return stream.ToString();
            }
            
        }

        private static void _WriteBinrary<T>(T obj, string path)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create);

            formatter.Serialize(fs, obj);

            fs.Close();
        }

        static public T Read<T>(byte[] stream)
        {
            //return _ReadBinraryStream<T>(stream);
            return _ReadXmlStream<T>(stream);
            
        }

        private static T _ReadXmlStream<T>(byte[] buffer)
        {
            
            
            using ( var stream = new System.IO.StringReader(System.Text.Encoding.Default.GetString(buffer)))
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
                return (T)ser.Deserialize(stream);
            }            
        }

        private static T _ReadBinraryStream<T>(byte[] stream)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formmater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            System.IO.MemoryStream ms = new System.IO.MemoryStream(stream);

            T obj = (T)formmater.Deserialize(ms);

            ms.Close();
            return obj;
        }
		static public T Read<T>(string path)
		{
            //return _ReadBinrary<T>(path);

            return _ReadXmlStream<T>(System.IO.File.ReadAllBytes(path));

			
		}

        private static T _ReadBinrary<T>(string path)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formmater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open);

            T obj = (T)formmater.Deserialize(fs);

            fs.Close();
            return obj;
        }
		
	}
}
