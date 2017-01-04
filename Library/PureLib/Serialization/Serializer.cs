using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using Regulus.Extension;

namespace Regulus.Serialization
{
    public class Serializer 
    {
        private readonly Type[] _Types;

        public Serializer(IEnumerable<Type> types)
        {
            _Types = types.ToArray();
        }

        public byte[] Serialize<T>(T data)
        {
            var type = typeof (T);
            var typeId = _Find(type);

            if (type.IsValueType)
            {
                var value = _SerializeValue(data);
            }

            return null;
        }
        

        private byte[] _SerializeValue(object data)
        {

            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {                
                bf.Serialize(ms, data);
                return ms.ToArray();
            }

        }

        private int _Find(Type type)
        {
            return _Types.Index(t => t == type);
        }
    }
}
