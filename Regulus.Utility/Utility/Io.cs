using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;




namespace Regulus.Utility
{
    public static class Serialization
    {
        public static void Write<T>(T obj, string path)
        {
            // System.IO.File.WriteAllBytes(path, _WriteProtobuf<T>(obj));
            File.WriteAllText(path, Serialization._WriteXml(obj));
        }


        public static string WriteXml<T>(T obj)
        {
            return _WriteXml<T>(obj);
        }
        private static string _WriteXml<T>(T obj)
        {
            using (StringWriter stream = new StringWriter())
            {
                XmlSerializer x = new XmlSerializer(typeof(T));
                x.Serialize(stream, obj);
                return stream.ToString();
            }
        }

        private static void _WriteBinrary<T>(T obj, string path)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream fs = new FileStream(path, FileMode.Create);

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
            using (StringReader stream = new StringReader(Encoding.Default.GetString(buffer)))
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                return (T)ser.Deserialize(stream);
            }
        }

        private static T _ReadBinraryStream<T>(byte[] stream)
        {
            BinaryFormatter formmater = new BinaryFormatter();

            MemoryStream ms = new MemoryStream(stream);

            T obj = (T)formmater.Deserialize(ms);

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
            BinaryFormatter formmater = new BinaryFormatter();

            FileStream fs = new FileStream(path, FileMode.Open);

            T obj = (T)formmater.Deserialize(fs);

            fs.Close();
            return obj;
        }
    }
}
